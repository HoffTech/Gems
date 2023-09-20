// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Handlers.FireJobImmediately;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Mvc.GenericControllers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;

namespace Gems.Jobs.Quartz
{
    /// <summary>
    /// Class with middleware extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Class with middleware extensions.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="configureOptions">Quartz options.</param>
        public static void AddQuartzWithJobs(this IServiceCollection services, IConfiguration configuration, Action<JobsOptions> configureOptions = null)
        {
            services.Configure<JobsOptions>(configuration.GetSection(JobsOptions.Jobs));
            var jobsOptions = configuration.GetSection(JobsOptions.Jobs).Get<JobsOptions>();
            if (jobsOptions == null)
            {
                return;
            }

            configureOptions?.Invoke(jobsOptions);
            if (jobsOptions.Triggers == null || jobsOptions.Triggers.Count == 0 || string.IsNullOrWhiteSpace(jobsOptions.ConnectionString))
            {
                return;
            }

            services.AddQuartz(q =>
            {
                q.SchedulerId = "AUTO";
                q.SchedulerName = jobsOptions.SchedulerName;

                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseTimeZoneConverter();

                q.UsePersistentStore(x =>
                {
                    x.UseProperties = true;
                    x.UseClustering(c =>
                    {
                        c.CheckinMisfireThreshold = TimeSpan.FromSeconds(60);
                        c.CheckinInterval = TimeSpan.FromSeconds(20);
                    });

                    switch (jobsOptions.Type)
                    {
                        case QuartzDbType.PostgreSql:
                            x.UsePostgres(postgres =>
                            {
                                postgres.TablePrefix = jobsOptions.TablePrefix;
                                postgres.ConnectionString = jobsOptions.ConnectionString;
                            });
                            break;
                        case QuartzDbType.MsSql:
                            x.UseSqlServer(sqlServer =>
                            {
                                sqlServer.TablePrefix = jobsOptions.TablePrefix;
                                sqlServer.ConnectionString = jobsOptions.ConnectionString;
                            });
                            break;
                        default:
                            throw new NotImplementedException($"Quartz not implemented for type: {jobsOptions.Type}");
                    }

                    x.UseJsonSerializer();
                });

                q.SetProperty("quartz.threadPool.type", "Quartz.Simpl.DefaultThreadPool, Quartz");
                q.SetProperty("quartz.threadPool.maxConcurrency", jobsOptions.MaxConcurrency?.ToString() ?? "25");

                foreach (var (jobType, jobName) in JobRegister.JobInfos)
                {
                    if (!jobsOptions.Triggers.TryGetValue(jobName, out var cronExpression))
                    {
                        continue;
                    }

                    var jobKey = new JobKey(jobName);

                    q.AddJob(jobType, jobKey, c => c.StoreDurably());

                    q.AddTrigger(t => t
                        .ForJob(jobKey)
                        .WithIdentity(jobName)
                        .WithDescription(jobName)
                        .WithCronSchedule(cronExpression));
                }
            });

            services.AddQuartzHostedService();
            services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<FireJobImmediatelyCommand>());
            ControllerRegister.RegisterControllers(typeof(FireJobImmediatelyCommand).Assembly);

            services.AddSingleton<SchedulerProvider>();
            services.AddHostedService<JobRecoveryHostedService>();
        }
    }
}
