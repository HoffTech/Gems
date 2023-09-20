// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Handlers.GetListOfJobs.Dto;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Mvc.GenericControllers;

using MediatR;

using Quartz;
using Quartz.Impl.Matchers;

namespace Gems.Jobs.Quartz.Handlers.GetListOfJobs;

[Endpoint("jobs/list", "GET", OperationGroup = "jobs", Summary = "Получить список заданий с триггерами")]
public class GetListOfJobsCommandHandler : IRequestHandler<GetListOfJobsCommand, List<JobInfo>>
{
    private readonly SchedulerProvider schedulerProvider;

    public GetListOfJobsCommandHandler(SchedulerProvider schedulerProvider)
    {
        this.schedulerProvider = schedulerProvider;
    }

    public async Task<List<JobInfo>> Handle(GetListOfJobsCommand request, CancellationToken cancellationToken)
    {
        var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);
        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup(), cancellationToken).ConfigureAwait(false);
        var jobsInfo = new List<JobInfo>();
        foreach (var jobKey in jobKeys)
        {
            var jobInfo = new JobInfo();
            var jobTriggers = await scheduler.GetTriggersOfJob(jobKey, cancellationToken).ConfigureAwait(false);
            var triggersInfo = new List<TriggerInfo>();
            foreach (var jobTrigger in jobTriggers)
            {
                var triggerInfo = new TriggerInfo { Trigger = jobTrigger };
                if (jobTrigger is ICronTrigger cronTrigger)
                {
                    triggerInfo.CronExpression = cronTrigger.CronExpressionString;
                }

                var triggerState = await scheduler
                    .GetTriggerState(jobTrigger.Key, cancellationToken)
                    .ConfigureAwait(false);
                triggerInfo.TriggerState = triggerState.ToString();

                triggersInfo.Add(triggerInfo);
            }

            jobInfo.JobKey = jobKey;
            jobInfo.Triggers = triggersInfo;
            jobsInfo.Add(jobInfo);
        }

        return jobsInfo;
    }
}
