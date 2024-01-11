// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Handlers.Consts;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Jobs.Quartz.Jobs.JobWithData;
using Gems.Text.Json;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Quartz;
using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz;

public class JobTriggerFromDbRegisterHostedService : BackgroundService
{
    private readonly SchedulerProvider schedulerProvider;
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly ILogger<JobTriggerFromDbRegisterHostedService> logger;
    private readonly IOptions<JobsOptions> jobsOptions;

    public JobTriggerFromDbRegisterHostedService(
        SchedulerProvider schedulerProvider,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<JobTriggerFromDbRegisterHostedService> logger,
        IOptions<JobsOptions> jobsOptions)
    {
        this.schedulerProvider = schedulerProvider;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.logger = logger;
        this.jobsOptions = jobsOptions;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await WaitForAppStartup(this.hostApplicationLifetime, stoppingToken))
        {
            return;
        }

        await this.RegisterTriggersFromConfiguration(stoppingToken).ConfigureAwait(false);
    }

    private static async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken stoppingToken)
    {
        var startedSource = new TaskCompletionSource();
        var cancelledSource = new TaskCompletionSource();

        await using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());
        await using var reg2 = stoppingToken.Register(() => cancelledSource.SetResult());

        var completedTask = await Task.WhenAny(
                                startedSource.Task,
                                cancelledSource.Task).ConfigureAwait(false);

        return completedTask == startedSource.Task;
    }

    private async Task RegisterTriggersByJobName(string jobName, CancellationToken cancellationToken)
    {
        var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
        if (this.jobsOptions.Value.TriggersFromDb == null || !this.jobsOptions.Value.TriggersFromDb.ContainsKey(jobName))
        {
            return;
        }

        foreach (var triggerFromDb in this.jobsOptions.Value.TriggersFromDb.GetValueOrDefault(jobName))
        {
            var triggerProviderType = Assembly.GetEntryAssembly()?.GetTypes()
                .Where(x => typeof(ITriggerDataProvider).IsAssignableFrom(x) && x.Name.Contains(triggerFromDb.ProviderType))
                .Select(Activator.CreateInstance)
                .Cast<ITriggerDataProvider>()
                .FirstOrDefault();
            if (triggerProviderType == null)
            {
                this.logger.LogWarning(
                    "Для триггера {triggerNameFromDb}, тип {triggerProviderType} не был найден или не реализует интерфейс ITriggerDataProvider",
                    triggerFromDb.TriggerName,
                    triggerFromDb.ProviderType);
                continue;
            }

            var cronExpression = await triggerProviderType.GetCronExpression(triggerFromDb.TriggerName, cancellationToken).ConfigureAwait(false);
            var triggerDataDict = await triggerProviderType.GetTriggerData(triggerFromDb.TriggerName, cancellationToken).ConfigureAwait(false);
            var jobKey = new JobKey(jobName);
            var newTrigger = new CronTriggerImpl(
                                 triggerFromDb.TriggerName ?? jobName,
                                 JobGroups.DefaultGroup,
                                 jobName,
                                 JobGroups.DefaultGroup)
            {
                JobKey = jobKey,
                Description = jobName
            };

            if (!string.IsNullOrEmpty(cronExpression))
            {
                newTrigger.CronExpressionString = cronExpression;
            }

            if (triggerDataDict != null && triggerDataDict.Any())
            {
                newTrigger.JobDataMap = new JobDataMap { [QuartzJobWithDataConstants.JobDataKeyValue] = triggerDataDict.Serialize() };
            }

            var existedTrigger = await scheduler.GetTrigger(newTrigger.Key, cancellationToken).ConfigureAwait(false);
            if (existedTrigger != null)
            {
                await scheduler.RescheduleJob(existedTrigger.Key, newTrigger, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private async Task RegisterTriggersFromConfiguration(CancellationToken cancellationToken)
    {
        await this.UnscheduleJobs(cancellationToken).ConfigureAwait(false);
        foreach (var (_, jobName) in JobRegister.JobNameByJobTypeMap)
        {
            await this.RegisterTriggersByJobName(jobName, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Метод для удаления триггеров из базы, которые отсутствуют в конфигурации.
    /// Триггеры ищутся по имени джоба, к которому они привязаны.
    /// </summary>
    private async Task UnscheduleJobs(CancellationToken cancellationToken)
    {
        var triggersFromConfig = new List<string>();
        if (this.jobsOptions.Value.Triggers != null)
        {
            triggersFromConfig.AddRange(this.jobsOptions.Value.Triggers.Select(t => t.Key));
        }

        if (this.jobsOptions.Value.TriggersWithData != null)
        {
            foreach (var triggerWithData in this.jobsOptions.Value.TriggersWithData.Values)
            {
                triggersFromConfig.AddRange(triggerWithData.Select(t => t.TriggerName));
            }
        }

        if (this.jobsOptions.Value.TriggersFromDb != null)
        {
            foreach (var triggerWithData in this.jobsOptions.Value.TriggersFromDb.Values)
            {
                triggersFromConfig.AddRange(triggerWithData.Select(t => t.TriggerName));
            }
        }

        var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
        var jobTriggersListToUnschedule = new List<ITrigger>();
        foreach (var (jobType, jobName) in JobRegister.JobNameByJobTypeMap)
        {
            var jobKey = new JobKey(jobName);
            var jobTriggers = (await scheduler.GetTriggersOfJob(jobKey, cancellationToken).ConfigureAwait(false)).ToList();
            jobTriggersListToUnschedule.AddRange(jobTriggers.Where(trigger => !triggersFromConfig.Contains(trigger.Key.Name)));
        }

        await scheduler.UnscheduleJobs(jobTriggersListToUnschedule.Select(j => j.Key).ToList(), cancellationToken).ConfigureAwait(false);
    }
}
