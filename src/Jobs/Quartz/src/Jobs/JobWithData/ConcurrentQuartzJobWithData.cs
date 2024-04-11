// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gems.Jobs.Quartz.Jobs.JobWithData;

public sealed class ConcurrentQuartzJobWithData<TCommand> : QuartzJobWithDataBase<TCommand> where TCommand : class, IRequest, new()
{
    public ConcurrentQuartzJobWithData(
        IMediator mediator,
        ILogger<ConcurrentQuartzJobWithData<TCommand>> logger,
        IConfiguration configuration)
        : base(mediator, logger, configuration)
    {
    }
}
