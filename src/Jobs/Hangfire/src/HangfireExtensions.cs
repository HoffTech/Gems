// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Storage;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Jobs.Hangfire
{
    public static class HangfireExtensions
    {
        public static IServiceCollection AddHangfire(
            this IServiceCollection services,
            string connectionString,
            int workerCount = 1)
        {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(
                    connectionString,
                    new PostgreSqlStorageOptions
                    {
                        SchemaName = "hangfire",
                        QueuePollInterval = TimeSpan.FromMilliseconds(1000),
                        InvisibilityTimeout = TimeSpan.FromMinutes(720),
                    }));

            services.AddHangfireServer(options => options.WorkerCount = workerCount);
            services.AddSingleton<HangfireWorker>();
            services.AddSingleton<IHangfireEnqueueManager, HangfireEnqueueManager>();

            return services;
        }

        public static IEndpointRouteBuilder MapHangfireDashboard(
            this IEndpointRouteBuilder endpoints,
            string dashboardRoot = "/dashboard",
            string serviceRoot = null)
        {
            endpoints.MapHangfireDashboard(dashboardRoot, new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthFilter() },
                PrefixPath = string.IsNullOrEmpty(serviceRoot) ? null : "/" + serviceRoot,
                IgnoreAntiforgeryToken = true,
            });

            return endpoints;
        }

        public static IApplicationBuilder AddHangfireJobsFromAssemblyContaining<T>(
            this IApplicationBuilder app,
            Func<string, string> mapCronExpression)
        {
            // Collect jobs.
            var assembly = typeof(T).Assembly;
            var handlerType = typeof(IRequestHandler<>);

            var jobs = assembly
                .GetTypes()
                .Select(x => new
                {
                    HandlerType = x,
                    HandlerInterface = x.GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .FirstOrDefault(i => i.GetGenericTypeDefinition() == handlerType),
                })
                .Where(x => x.HandlerInterface != null)
                .SelectMany(x => x.HandlerType.GetCustomAttributes(false)
                    .Where(a => a is JobHandlerAttribute)
                    .Select(a => new
                    {
                        CommandType = x.HandlerInterface.GetGenericArguments().First(),
                        Name = (a as JobHandlerAttribute).Name,
                        ChronExpression = mapCronExpression((a as JobHandlerAttribute).Name),
                    }))
                .Where(x => !string.IsNullOrEmpty(x.ChronExpression))
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .ToList();

            // Remove invalid jobs.
            var validJobs = jobs.Select(x => x.Name).ToHashSet();
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    if (!validJobs.Contains(recurringJob.Id))
                    {
                        RecurringJob.RemoveIfExists(recurringJob.Id);
                    }
                }
            }

            // Register or update jobs.
            jobs.ForEach(job =>
            {
                // Строим expression-tree для сериализации в Hangfire
                var typeHangfireWorker = typeof(HangfireWorker);
                var workerParam = Expression.Parameter(typeof(HangfireWorker), "worker");
                var runMethod = typeHangfireWorker
                    .GetMethod("Run")
                    .MakeGenericMethod(job.CommandType);
                var jobNameConst = Expression.Constant(job.Name);
                var jobCancellationTokenNone = Expression.Constant(CancellationToken.None);
                var jobCommandType = Expression.Constant(job.CommandType);
                var methodCreateInstance = typeof(Activator).GetMethod(
                    "CreateInstance",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                    null,
                    new Type[] { typeof(Type) },
                    null);
                var createInstanceMethod = Expression.Convert(
                    Expression.Call(null, methodCreateInstance, jobCommandType), job.CommandType);
                var body = Expression.Call(
                    workerParam,
                    runMethod,
                    jobNameConst,
                    createInstanceMethod,
                    jobCancellationTokenNone);
                var actionExpression = Expression.Lambda<Func<HangfireWorker, Task>>(body, workerParam);

                // Регистрируем задание.
                RecurringJob.AddOrUpdate(
                    job.Name,
                    actionExpression,
                    job.ChronExpression);
            });

            return app;
        }
    }
}
