// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Jobs.Quartz.Handlers.GetListOfJobs.Dto;
using Gems.Jobs.Quartz.Handlers.Shared;
using Gems.Jobs.Quartz.Handlers.Shared.JobsInfoProvider;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Jobs.Quartz.Handlers.GetListOfJobs;

[Endpoint("jobs/list", "GET", OperationGroup = "jobs", Summary = "Получить список заданий с триггерами")]
public class GetListOfJobsCommandHandler : IRequestHandler<GetListOfJobsCommand, List<JobInfo>>
{
    private readonly SchedulerProvider schedulerProvider;
    private readonly JobsInfoProvider jobsInfoProvider;
    private readonly IMapper mapper;

    public GetListOfJobsCommandHandler(SchedulerProvider schedulerProvider, JobsInfoProvider jobsInfoProvider, IMapper mapper)
    {
        this.schedulerProvider = schedulerProvider;
        this.jobsInfoProvider = jobsInfoProvider;
        this.mapper = mapper;
    }

    public async Task<List<JobInfo>> Handle(GetListOfJobsCommand request, CancellationToken cancellationToken)
    {
        var scheduler = await this.schedulerProvider.GetSchedulerAsync(cancellationToken).ConfigureAwait(false);

        return this.mapper.Map<List<JobInfo>>(await this.jobsInfoProvider.GetJobsInfo(scheduler, cancellationToken).ConfigureAwait(false));
    }
}
