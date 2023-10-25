// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;

using Microsoft.Extensions.Hosting;
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

        public BlockedJobsRecoveryHostedService(IOptions<JobsOptions> options)
        {
            this.options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var scheduler = await SchedulerRepository.Instance
                .Lookup(this.options.Value.SchedulerName, cancellationToken)
                .ConfigureAwait(false);
            if (scheduler is null)
            {
                throw new ArgumentNullException(
                    nameof(scheduler),
                    $"Can't find Scheduler with name \"{this.options.Value.SchedulerName}\"");
            }

            while (true)
            {
                var blockedJobsRecoveryOptions = this.options.Value.BlockedJobsRecovery;
                var workersToRecover = blockedJobsRecoveryOptions.WorkersToRecover;
                var maxDelayBetweenLastFireTimeAndRecoverTime = new TimeSpan(blockedJobsRecoveryOptions
                    .MaxDelayBetweenLastFireTimeAndRecoverTimeInMilliseconds);

                if (workersToRecover != null && workersToRecover.Count > 0)
                {
                    var triggerKeys = await scheduler
                        .GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup(), cancellationToken)
                        .ConfigureAwait(false);

                    foreach (var key in triggerKeys)
                    {
                        var triggerState = await scheduler.GetTriggerState(key, cancellationToken).ConfigureAwait(false);
                        if (triggerState is TriggerState.Blocked)
                        {
                            var trigger = await scheduler.GetTrigger(key, cancellationToken).ConfigureAwait(false);
                            if (trigger != null && workersToRecover.Contains(trigger.JobKey.Name))
                            {
                                var lastFireTime = trigger.GetPreviousFireTimeUtc();
                                var needToRecover = (DateTime.UtcNow - lastFireTime) > maxDelayBetweenLastFireTimeAndRecoverTime;
                                if (needToRecover
                                    && trigger is CronTriggerImpl cronTrigger
                                    && !string.IsNullOrWhiteSpace(cronTrigger.CronExpressionString))
                                {
                                    var newTrigger = (CronTriggerImpl)cronTrigger.Clone();
                                    newTrigger.CronExpression = new CronExpression(cronTrigger.CronExpressionString);

                                    await scheduler.UnscheduleJob(trigger.Key, cancellationToken).ConfigureAwait(false);
                                    await scheduler.ScheduleJob(newTrigger, cancellationToken).ConfigureAwait(false);
                                }
                            }
                        }
                    }
                }

                await Task.Delay(blockedJobsRecoveryOptions.CheckIntervalInMilliseconds, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
