// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Handlers.FireJobImmediately;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Mvc.GenericControllers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;
using Quartz.Impl;

using Quartzmon;

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
            services.AddSingleton<SchedulerProvider>();

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
                if (jobsOptions.BatchTriggerAcquisitionMaxCount != null)
                {
                    q.SetProperty("quartz.scheduler.batchTriggerAcquisitionMaxCount", jobsOptions.BatchTriggerAcquisitionMaxCount.ToString());
                }

                if (jobsOptions.AcquireTriggersWithinLock != null)
                {
                    q.SetProperty("quartz.jobStore.acquireTriggersWithinLock", jobsOptions.AcquireTriggersWithinLock.ToString().ToLower());
                }

                if (jobsOptions.EnableAdminUiPersistentJobHistory)
                {
                    q.SetProperty("quartz.plugin.recentHistory.type", "Quartzmon.Plugins.RecentHistory.PostgreSql.PostgreSqlExecutionHistoryPlugin, Quartzmon.Plugins.RecentHistory.PostgreSql");
                    q.SetProperty("quartz.plugin.recentHistory.storeType", "Quartzmon.Plugins.RecentHistory.PostgreSql.Store.PostgreSqlExecutionHistoryStore, Quartzmon.Plugins.RecentHistory.PostgreSql");
                    q.SetProperty("quartz.plugin.recentHistory.connectionString", jobsOptions.ConnectionString);
                    q.SetProperty("quartz.plugin.recentHistory.entryTtlInMinutes", (jobsOptions.PersistentRecentHistoryEntryTtl ?? 60).ToString());
                    q.SetProperty("quartz.plugin.recentHistory.tablePrefix", jobsOptions.TablePrefix);
                }

                QuartzPropertiesSetter.SetProperties(q, jobsOptions.QuartzProperties);

                foreach (var (jobType, jobName) in JobRegister.JobNameByJobTypeMap)
                {
                    var cronExpression = jobsOptions.Triggers.GetValueOrDefault(jobName);

                    var jobKey = new JobKey(jobName);

                    q.AddJob(jobType, jobKey, c => c.StoreDurably());

                    q.AddTrigger(tConf =>
                    {
                        var configuredTrigger = tConf
                            .ForJob(jobKey)
                            .WithIdentity(jobName)
                            .WithDescription(jobName);
                        if (!string.IsNullOrWhiteSpace(cronExpression))
                        {
                            configuredTrigger.WithCronSchedule(cronExpression);
                        }
                    });
                }
            });

            services.AddQuartzHostedService();

            ControllerRegister.RegisterControllers(Assembly.GetExecutingAssembly());
            services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<FireJobImmediatelyCommand>());

            services.AddQuartzmon();

            services.AddHostedService<JobRecoveryHostedService>();
            services.AddHostedService<BlockedJobsRecoveryHostedService>();
        }

        public static IApplicationBuilder UseQuartzAdminUI(this IApplicationBuilder app, IConfiguration configuration)
        {
            var jobsOptions = configuration.GetSection(JobsOptions.Jobs).Get<JobsOptions>();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments(jobsOptions.AdminUiUrl))
                {
                    var syncIoFeature = context.Features.Get<IHttpBodyControlFeature>();
                    if (syncIoFeature != null)
                    {
                        syncIoFeature.AllowSynchronousIO = true;
                    }
                }

                await next();
            });
            app.UseQuartzmon(
                new QuartzmonOptions
                {
                    VirtualPathRoot = jobsOptions.AdminUiUrl,
                    UrlPartPrefix = jobsOptions.AdminUiUrlPrefix,
                    DefaultDateFormat = "dd.MM.yyyy",
                    DefaultTimeFormat = "HH:mm:ss"
                },
                services =>
                {
                    var scheduler = SchedulerRepository.Instance.Lookup(jobsOptions.SchedulerName, CancellationToken.None).GetAwaiter().GetResult();
                    services.Scheduler = scheduler;
                });

            return app;
        }
    }
}
