// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;

using Microsoft.Extensions.Options;

using Quartz;
using Quartz.Impl;

namespace Gems.Jobs.Quartz.Handlers.Shared
{
    public class SchedulerProvider
    {
        private readonly IOptions<JobsOptions> options;

        public SchedulerProvider(IOptions<JobsOptions> options)
        {
            this.options = options;
        }

        public async Task<IScheduler> GetSchedulerAsync(CancellationToken cancellationToken)
        {
            var scheduler = await SchedulerRepository.Instance
                .Lookup(this.options.Value.SchedulerName, cancellationToken)
                .ConfigureAwait(false);

            return scheduler ?? throw new ArgumentNullException(
                       nameof(scheduler),
                       $"Can't find Scheduler with name \"{this.options.Value.SchedulerName}\"");
        }
    }
}
