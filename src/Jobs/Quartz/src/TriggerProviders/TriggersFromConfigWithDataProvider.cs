// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Handlers.Consts;
using Gems.Linq;

using Microsoft.Extensions.Options;

using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz.TriggerProviders;

public class TriggersFromConfigWithDataProvider : ITriggerProvider
{
    private readonly IOptions<JobsOptions> jobsOptions;

    public TriggersFromConfigWithDataProvider(IOptions<JobsOptions> jobsOptions)
    {
        this.jobsOptions = jobsOptions;
    }

    public Task<List<CronTriggerImpl>> GetTriggers(string jobName, CancellationToken cancellationToken)
    {
        var result = new List<CronTriggerImpl>();

        if (this.jobsOptions.Value?.TriggersWithData.IsNullOrEmpty() ?? true)
        {
            return Task.FromResult(result);
        }

        foreach (var triggerWithData in this.jobsOptions.Value?.TriggersWithData?.Values?
                     .SelectMany(t => t
                         .Where(o => o.TriggerName == jobName)
                         .Select(o => o)))
        {
            result.Add(TriggerHelper.CreateCronTrigger(
                triggerWithData.TriggerName,
                JobGroups.DefaultGroup,
                jobName,
                JobGroups.DefaultGroup,
                triggerWithData.CronExpression,
                triggerWithData.TriggerData));
        }

        return Task.FromResult(result);
    }
}
