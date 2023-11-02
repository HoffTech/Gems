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

namespace Gems.Jobs.Quartz
{
    public class JobRecoveryHostedService : BackgroundService
    {
        private readonly IOptions<JobsOptions> options;
        private readonly ILogger<JobRecoveryHostedService> logger;

        public JobRecoveryHostedService(
            IOptions<JobsOptions> options,
            ILogger<JobRecoveryHostedService> logger)
        {
            this.options = options;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
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
                    var triggerKeys = await scheduler
                        .GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup(), cancellationToken)
                        .ConfigureAwait(false);

                    foreach (var key in triggerKeys)
                    {
                        var triggerState =
                            await scheduler.GetTriggerState(key, cancellationToken).ConfigureAwait(false);
                        if (triggerState is TriggerState.Error)
                        {
                            await scheduler.ResetTriggerFromErrorState(key, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, "Recovery of jobs with errors failed");
                }
                finally
                {
                    await Task.Delay(this.options.Value.JobRecoveryDelayInMilliseconds, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
