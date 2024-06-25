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

public class TriggersFromDbProvider : ITriggerProvider
{
    private readonly TriggerHelper triggerHelper;
    private readonly IOptions<JobsOptions> jobsOptions;

    public TriggersFromDbProvider(
        TriggerHelper triggerHelper,
        IOptions<JobsOptions> jobsOptions)
    {
        this.triggerHelper = triggerHelper;
        this.jobsOptions = jobsOptions;
    }

    public async Task<List<CronTriggerImpl>> GetTriggers(string jobName, CancellationToken cancellationToken)
    {
        var result = new List<CronTriggerImpl>();

        if ((this.jobsOptions.Value?.TriggersFromDb.IsNullOrEmpty() ?? true)
            || !(this.jobsOptions.Value?.TriggersFromDb?.ContainsKey(jobName) ?? false))
        {
            return result;
        }

        foreach (var triggerFromDbOpt in this.jobsOptions.Value.TriggersFromDb?.Values?
                     .SelectMany(t => t
                         .Select(o => o)))
        {
            result.Add(await this.triggerHelper.GetTriggerFromDb(jobName, JobGroups.DefaultGroup, triggerFromDbOpt.CronExpression, triggerFromDbOpt, cancellationToken));
        }

        return result;
    }
}
