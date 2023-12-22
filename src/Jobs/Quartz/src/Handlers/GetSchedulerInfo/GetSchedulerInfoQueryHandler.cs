// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Jobs.Quartz.Handlers.GetSchedulerInfo.Dto;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Jobs.Quartz.Handlers.Shared.JobsInfoProvider;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Jobs.Quartz.Handlers.GetSchedulerInfo;

[Endpoint("jobs/scheduler", "GET", OperationGroup = "jobs", Summary = "Получить информацию о шедулере")]
public class GetSchedulerInfoQueryHandler : IRequestHandler<GetSchedulerInfoQuery, SchedulerInfo>
{
    private readonly SchedulerProvider schedulerProvider;
    private readonly JobsInfoProvider jobsInfoProvider;
    private readonly IMapper mapper;

    public GetSchedulerInfoQueryHandler(SchedulerProvider schedulerProvider, JobsInfoProvider jobsInfoProvider, IMapper mapper)
    {
        this.schedulerProvider = schedulerProvider;
        this.jobsInfoProvider = jobsInfoProvider;
        this.mapper = mapper;
    }

    public async Task<SchedulerInfo> Handle(GetSchedulerInfoQuery request, CancellationToken cancellationToken)
    {
        var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);

        return new SchedulerInfo
        {
            SchedulerName = scheduler.SchedulerName,
            SchedulerInstanceId = scheduler.SchedulerInstanceId,
            EnqueuedJobList = this.mapper.Map<List<JobInfo>>(await this.jobsInfoProvider.GetJobsInfo(scheduler, cancellationToken).ConfigureAwait(false))
        };
    }
}
