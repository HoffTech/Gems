// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz
{
    public class BlockedJobsRecoveryHostedService : BackgroundService
    {
        private readonly IOptions<JobsOptions> options;
        private readonly ILogger<BlockedJobsRecoveryHostedService> logger;

        public BlockedJobsRecoveryHostedService(
            IOptions<JobsOptions> options,
            ILogger<BlockedJobsRecoveryHostedService> logger)
        {
            this.options = options;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (this.options.Value.BlockedJobsRecovery == null)
            {
                return;
            }

            var scheduler = await SchedulerRepository.Instance
                .Lookup(this.options.Value.SchedulerName, cancellationToken)
                .ConfigureAwait(false);
            if (scheduler is null)
            {
                this.logger.LogWarning("Can't find Scheduler with name \"{SchedulerName}\"", this.options.Value.SchedulerName);
                return;
            }

            while (true)
            {
                try
                {
                    var blockedJobsRecoveryOptions = this.options.Value.BlockedJobsRecovery;
                    var workersToRecover = blockedJobsRecoveryOptions.WorkersToRecover;
                    var maxDelayBetweenLastFireTimeAndRecoverTime = new TimeSpan(blockedJobsRecoveryOptions
                        .MaxDelayBetweenLastFireTimeAndRecoverTimeInMilliseconds);

                    if (workersToRecover is not { Count: > 0 })
                    {
                        continue;
                    }

                    var triggerKeys = await scheduler
                                          .GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup(), cancellationToken)
                                          .ConfigureAwait(false);

                    foreach (var key in triggerKeys)
                    {
                        var triggerState = await scheduler.GetTriggerState(key, cancellationToken)
                                               .ConfigureAwait(false);
                        if (triggerState is not TriggerState.Blocked)
                        {
                            continue;
                        }

                        var trigger = await scheduler.GetTrigger(key, cancellationToken).ConfigureAwait(false);
                        if (trigger == null || !workersToRecover.Contains(trigger.JobKey.Name))
                        {
                            continue;
                        }

                        var lastFireTime = trigger.GetPreviousFireTimeUtc() ?? DateTimeOffset.MinValue;
                        var needToRecover = (DateTime.UtcNow - lastFireTime) >
                                            maxDelayBetweenLastFireTimeAndRecoverTime;
                        this.logger.LogInformation("Trigger ({TriggerKey}) fired at ({TriggerLastFireTime}). Should it be recovered? ({TriggerShouldBeRecovered})", trigger.JobKey.Name, lastFireTime, needToRecover);
                        if (needToRecover
                            && trigger is CronTriggerImpl cronTrigger
                            && !string.IsNullOrWhiteSpace(cronTrigger.CronExpressionString))
                        {
                            var newTrigger = (CronTriggerImpl)cronTrigger.Clone();
                            newTrigger.CronExpression =
                                new CronExpression(cronTrigger.CronExpressionString);

                            var unscheduledResult = await scheduler.UnscheduleJob(trigger.Key, cancellationToken)
                                                       .ConfigureAwait(false);
                            if (unscheduledResult)
                            {
                                await scheduler.ScheduleJob(newTrigger, cancellationToken)
                                    .ConfigureAwait(false);
                                this.logger.LogInformation("Trigger ({TriggerKey}) successfully rescheduled", trigger.JobKey.Name);
                            }
                            else
                            {
                                this.logger.LogInformation("Trigger ({TriggerKey}) failed to unschedule", trigger.JobKey.Name);
                            }
                        }
                        else
                        {
                            this.logger.LogInformation("Trigger ({TriggerKey}) shouldn't be recovered", trigger.JobKey.Name);
                        }
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, "Recovery of blocked jobs failed");
                }
                finally
                {
                    await Task.Delay(this.options.Value.BlockedJobsRecovery.CheckIntervalInMilliseconds, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
