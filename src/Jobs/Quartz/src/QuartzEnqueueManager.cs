using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Jobs.JobWithData;
using Gems.Text.Json;

using MediatR;

using Microsoft.Extensions.Options;

using Quartz;
using Quartz.Impl;

namespace Gems.Jobs.Quartz;

public class QuartzEnqueueManager : IQuartzEnqueueManager
{
    private const string DefaultGroup = "DEFAULT";
    private readonly IOptions<JobsOptions> options;

    public QuartzEnqueueManager(IOptions<JobsOptions> options)
    {
        this.options = options;
    }

    public async Task Enqueue<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, IRequest, new()
    {
        var jobName = GetJobName<TCommand>();
        var serializedCommand = command.Serialize();
        var jobData = new JobDataMap { [QuartzJobWithDataConstants.JobDataKeyValue] = serializedCommand };
        var scheduler = await SchedulerRepository.Instance.Lookup(this.options.Value.SchedulerName, cancellationToken);
        await scheduler.TriggerJob(new JobKey(jobName, DefaultGroup), jobData, cancellationToken);
    }

    private static string GetJobName<TCommand>()
    {
        var jobName = JobRegister.JobNameByRequestTypeMap.GetValueOrDefault(typeof(TCommand));
        if (string.IsNullOrWhiteSpace(jobName))
        {
            throw new ArgumentException($"There is no job associated with command {typeof(TCommand)}");
        }

        return jobName;
    }
}
