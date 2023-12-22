// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoMapper;

using Gems.Jobs.Quartz.Handlers.GetListOfJobs.Dto;

namespace Gems.Jobs.Quartz.Handlers.GetListOfJobs;

public class GetListOfJobsMapper : Profile
{
    public GetListOfJobsMapper()
    {
        this.CreateMap<Shared.JobsInfoProvider.Dto.JobInfo, JobInfo>();
        this.CreateMap<Shared.JobsInfoProvider.Dto.TriggerInfo, TriggerInfo>();
    }
}
