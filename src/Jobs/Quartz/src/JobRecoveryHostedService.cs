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
    public class JobRecoveryHostedService : BackgroundService
    {
        private readonly IOptions<JobsOptions> options;

        public JobRecoveryHostedService(IOptions<JobsOptions> options)
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
                var triggerKeys = await scheduler
                    .GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup(), cancellationToken)
                    .ConfigureAwait(false);
                var workersToRecoverFromBlockedState = this.options.Value.WorkersToRecoverFromBlockedState;

                foreach (var key in triggerKeys)
                {
                    var triggerState = await scheduler.GetTriggerState(key, cancellationToken).ConfigureAwait(false);
                    if (triggerState is TriggerState.Error)
                    {
                        await scheduler.ResetTriggerFromErrorState(key, cancellationToken).ConfigureAwait(false);
                    }

                    if (triggerState is TriggerState.Blocked
                        && workersToRecoverFromBlockedState != null && workersToRecoverFromBlockedState.Count > 0)
                    {
                        var trigger = await scheduler.GetTrigger(key, cancellationToken).ConfigureAwait(false);
                        if (trigger != null
                            && workersToRecoverFromBlockedState.Contains(trigger.JobKey.Name)
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

                await Task.Delay(this.options.Value.JobRecoveryDelayInMilliseconds, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
