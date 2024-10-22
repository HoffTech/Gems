// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Reflection;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Handlers.FireJobImmediately;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Jobs.Quartz.TriggerProviders;
using Gems.Jobs.Quartz.TriggerValidators;
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
        /// <param name="configurePlugins">Set custom persistent store properties.</param>
        public static void AddQuartzWithJobs(this IServiceCollection services, IConfiguration configuration, Action<JobsOptions> configureOptions = null, Action<IPropertyConfigurer, JobsOptions> configurePlugins = null)
        {
            services.AddSingleton<SchedulerProvider>();
            services.AddSingleton<TriggerHelper>();

            services.Configure<JobsOptions>(configuration.GetSection(JobsOptions.Jobs));
            var jobsOptions = configuration.GetSection(JobsOptions.Jobs).Get<JobsOptions>();
            if (jobsOptions == null)
            {
                return;
            }

            configureOptions?.Invoke(jobsOptions);
            if (jobsOptions.Type != QuartzDbType.InMemory && string.IsNullOrWhiteSpace(jobsOptions.ConnectionString))
            {
                return;
            }

            services.AddQuartz(q =>
            {
                q.SchedulerId = "AUTO";
                q.SchedulerName = jobsOptions.SchedulerName;

                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseTimeZoneConverter();

                if (jobsOptions.Type == QuartzDbType.InMemory)
                {
                    q.UseInMemoryStore();
                }
                else
                {
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
                }

                configurePlugins?.Invoke(q, jobsOptions);

                q.SetProperty("quartz.threadPool.type", "Quartz.Simpl.DefaultThreadPool, Quartz");
#pragma warning disable CS0618 // Type or member is obsolete
                q.SetProperty("quartz.threadPool.maxConcurrency", jobsOptions.MaxConcurrency?.ToString() ?? "25");
                if (jobsOptions.BatchTriggerAcquisitionMaxCount != null)
                {
                    q.SetProperty("quartz.scheduler.batchTriggerAcquisitionMaxCount", jobsOptions.BatchTriggerAcquisitionMaxCount.ToString());
                }

                if (jobsOptions.AcquireTriggersWithinLock != null)
                {
                    q.SetProperty("quartz.jobStore.acquireTriggersWithinLock", jobsOptions.AcquireTriggersWithinLock.ToString().ToLower());
                }
#pragma warning restore CS0618 // Type or member is obsolete
                QuartzPropertiesSetter.SetProperties(q, jobsOptions.QuartzProperties);

                foreach (var (jobType, jobName) in JobRegister.JobNameByJobTypeMap)
                {
                    var jobKey = new JobKey(jobName);
                    q.AddJob(jobType, jobKey, c => c.StoreDurably());
                }
            });

            services.AddQuartzHostedService();

            ControllerRegister.RegisterControllers(Assembly.GetExecutingAssembly());
            services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<FireJobImmediatelyCommand>());

            services.AddHostedService<JobRecoveryHostedService>();
            services.AddHostedService<BlockedJobsRecoveryHostedService>();
            services.AddHostedService<JobTriggerRegisterHostedService>();

            services.AddSingleton<StoredCronTriggerProvider>();

            services.AddSingleton<ITriggerProvider, TriggersFromConfigProvider>();
            services.AddSingleton<ITriggerProvider, TriggersFromConfigWithDataProvider>();
            services.AddSingleton<ITriggerProvider, TriggersFromDbProvider>();

            services.AddSingleton<ITriggerValidator, TriggerDuplicateValidator>();
        }
    }
}
