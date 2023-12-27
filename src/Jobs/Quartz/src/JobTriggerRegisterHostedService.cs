// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Consts;
using Gems.Jobs.Quartz.Handlers.Consts;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Jobs.Quartz.Jobs.JobWithData;
using Gems.Text.Json;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Quartz;
using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz;

public class JobTriggerRegisterHostedService : BackgroundService
{
    private readonly SchedulerProvider schedulerProvider;
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly IConfiguration configuration;

    public JobTriggerRegisterHostedService(
        SchedulerProvider schedulerProvider,
        IHostApplicationLifetime hostApplicationLifetime,
        IConfiguration configuration)
    {
        this.schedulerProvider = schedulerProvider;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await WaitForAppStartup(this.hostApplicationLifetime, stoppingToken))
        {
            return;
        }

        await this.RegisterTriggersFromConfiguration(this.configuration, stoppingToken).ConfigureAwait(false);
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

    private async Task RegisterTriggersFromConfiguration(IConfiguration configuration, CancellationToken cancellationToken)
    {
        await this.UnscheduleJobs(cancellationToken).ConfigureAwait(false);
        var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
        var jobsOptions = configuration.GetSection(JobsOptions.Jobs).Get<JobsOptions>();
        if (jobsOptions.TriggersOptions != null)
        {
            foreach (var triggerOption in jobsOptions.TriggersOptions.Where(triggerOption => jobsOptions.Triggers.ContainsKey(triggerOption.Key)))
            {
                jobsOptions.Triggers[triggerOption.Key] = triggerOption.Value;
            }
        }

        foreach (var (jobType, jobName) in JobRegister.JobNameByJobTypeMap)
        {
            var triggersOptions = await TriggerRegister.GetTriggerData(jobsOptions.Triggers.GetValueOrDefault(jobName)).ConfigureAwait(false);
            foreach (var triggersOption in triggersOptions)
            {
                var jobKey = new JobKey(jobName);
                var newTrigger = new CronTriggerImpl(
                                     triggersOption.TriggerName ?? jobName,
                                     JobGroups.DefaultGroup,
                                     jobName,
                                     JobGroups.DefaultGroup)
                {
                    JobKey = jobKey,
                    Description = jobName
                };

                if (!string.IsNullOrEmpty(triggersOption.CronExpression))
                {
                    newTrigger.CronExpressionString = triggersOption.CronExpression;
                }

                if (triggersOption.TriggerData != null)
                {
                    newTrigger.JobDataMap = new JobDataMap { [QuartzJobWithDataConstants.JobDataKeyValue] = triggersOption.TriggerData.Serialize() };
                }

                await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private async Task UnscheduleJobs(CancellationToken cancellationToken)
    {
        var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
        var jobTriggersList = new List<ITrigger>();
        foreach (var (jobType, jobName) in JobRegister.JobNameByJobTypeMap)
        {
            var jobKey = new JobKey(jobName);
            var jobTriggers = await scheduler.GetTriggersOfJob(jobKey, cancellationToken).ConfigureAwait(false);
            jobTriggersList.AddRange(jobTriggers);
        }

        await scheduler.UnscheduleJobs(jobTriggersList.Select(j => j.Key).ToList(), cancellationToken).ConfigureAwait(false);
    }
}
