// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Jobs.Quartz.TriggerProviders;
using Gems.Jobs.Quartz.TriggerValidators;
using Gems.Linq;
using Gems.Mvc.Filters.Exceptions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Quartz;
using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz;

public class JobTriggerRegisterHostedService : BackgroundService
{
    private readonly SchedulerProvider schedulerProvider;
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly IOptions<JobsOptions> jobsOptions;
    private readonly IEnumerable<ITriggerProvider> triggerProviderCollection;
    private readonly IEnumerable<ITriggerValidator> triggerValidator;
    private readonly StoredCronTriggerProvider storedCronTriggerProvider;

    public JobTriggerRegisterHostedService(
        SchedulerProvider schedulerProvider,
        IHostApplicationLifetime hostApplicationLifetime,
        IOptions<JobsOptions> jobsOptions,
        IEnumerable<ITriggerProvider> triggerProviderCollection,
        IEnumerable<ITriggerValidator> triggerValidator,
        StoredCronTriggerProvider storedCronTriggerProvider)
    {
        this.schedulerProvider = schedulerProvider;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.jobsOptions = jobsOptions;
        this.triggerProviderCollection = triggerProviderCollection;
        this.triggerValidator = triggerValidator;
        this.storedCronTriggerProvider = storedCronTriggerProvider;
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

    private static async Task ScheduleTrigger(IScheduler scheduler, ITrigger newTrigger, CancellationToken cancellationToken)
    {
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

    private async Task RegisterTriggers(IScheduler scheduler, List<CronTriggerImpl> triggerCollection, CancellationToken cancellationToken)
    {
        foreach (var trigger in triggerCollection)
        {
            await this.UpdateCronExpression(trigger, cancellationToken).ConfigureAwait(false);
            await ScheduleTrigger(scheduler, trigger, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<List<CronTriggerImpl>> CollectTriggers(CancellationToken cancellationToken)
    {
        var triggerCollection = new List<CronTriggerImpl>();

        foreach (var (_, jobName) in JobRegister.JobNameByJobTypeMap)
        {
            triggerCollection.AddRange(await this.CollectTriggersByJobName(jobName, cancellationToken).ConfigureAwait(false));
        }

        return triggerCollection;
    }

    private async Task UpdateCronExpression(CronTriggerImpl trigger, CancellationToken cancellationToken)
    {
        if (!this.jobsOptions.Value.EnablePersistenceStore)
        {
            return;
        }

        var cronExpression = await this.storedCronTriggerProvider.GetCronExpression(trigger.Name, cancellationToken);

        if (string.IsNullOrWhiteSpace(cronExpression))
        {
            await this.storedCronTriggerProvider.WriteCronExpression(trigger.Name, trigger.CronExpressionString, cancellationToken);
        }
        else
        {
            trigger.CronExpressionString = cronExpression;
        }
    }

    private async Task<List<CronTriggerImpl>> CollectTriggersByJobName(string jobName, CancellationToken cancellationToken)
    {
        var triggerCollection = new List<CronTriggerImpl>();

        foreach (var triggerProvider in this.triggerProviderCollection)
        {
            triggerCollection.AddRange(await triggerProvider.GetTriggers(jobName, cancellationToken).ConfigureAwait(false));
        }

        return triggerCollection;
    }

    private bool ValidateTriggers(List<CronTriggerImpl> triggerCollection)
    {
        if (triggerCollection.IsNullOrEmpty())
        {
            return false;
        }

        foreach (var validator in this.triggerValidator)
        {
            if (!validator.CheckIsValid(triggerCollection, out var errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }
        }

        return true;
    }

    private async Task RegisterTriggersFromConfiguration(CancellationToken cancellationToken)
    {
        var triggerCollection = await this.CollectTriggers(cancellationToken).ConfigureAwait(false);

        if (this.ValidateTriggers(triggerCollection) is false)
        {
            return;
        }

        var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);

        await this.UnscheduleJobs(scheduler, cancellationToken).ConfigureAwait(false);

        await this.RegisterTriggers(scheduler, triggerCollection, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Метод для удаления триггеров из расписания, которые отсутствуют в конфигурации.
    /// Триггеры ищутся по имени джоба, к которому они привязаны.
    /// </summary>
    private async Task UnscheduleJobs(IScheduler scheduler, CancellationToken cancellationToken)
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

        var jobTriggersListToUnschedule = new List<ITrigger>();
        foreach (var (_, jobName) in JobRegister.JobNameByJobTypeMap)
        {
            var jobKey = new JobKey(jobName);
            var jobTriggers = (await scheduler.GetTriggersOfJob(jobKey, cancellationToken).ConfigureAwait(false)).ToList();
            jobTriggersListToUnschedule.AddRange(jobTriggers.Where(trigger => !triggersFromConfig.Contains(trigger.Key.Name)));
        }

        await scheduler.UnscheduleJobs(jobTriggersListToUnschedule.Select(j => j.Key).ToList(), cancellationToken).ConfigureAwait(false);
    }
}
