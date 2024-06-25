// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Handlers.Consts;

using Microsoft.Extensions.Options;

using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz.TriggerProviders;

public class TriggersFromConfigProvider : ITriggerProvider
{
    private readonly IOptions<JobsOptions> jobsOptions;

    public TriggersFromConfigProvider(IOptions<JobsOptions> jobsOptions)
    {
        this.jobsOptions = jobsOptions;
    }

    public Task<List<CronTriggerImpl>> GetTriggers(string jobName, CancellationToken cancellationToken)
    {
        var result = this.jobsOptions.Value.Triggers?.Where(o => o.Key == jobName)
            .Select(o => o)
            .Select(trigger => TriggerHelper.CreateCronTrigger(trigger.Key, JobGroups.DefaultGroup, jobName, JobGroups.DefaultGroup, trigger.Value))
            .ToList();

        return Task.FromResult(result);
    }
}
