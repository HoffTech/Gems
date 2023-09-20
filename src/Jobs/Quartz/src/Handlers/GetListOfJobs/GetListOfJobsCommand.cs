// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Gems.Jobs.Quartz.Handlers.GetListOfJobs.Dto;

using MediatR;

namespace Gems.Jobs.Quartz.Handlers.GetListOfJobs;

public class GetListOfJobsCommand : IRequest<List<JobInfo>>
{
}
