// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Jobs.Quartz.Handlers.GetSchedulerInfo.Dto;

using MediatR;

namespace Gems.Jobs.Quartz.Handlers.GetSchedulerInfo;

public class GetSchedulerInfoQuery : IRequest<SchedulerInfo>
{
}
