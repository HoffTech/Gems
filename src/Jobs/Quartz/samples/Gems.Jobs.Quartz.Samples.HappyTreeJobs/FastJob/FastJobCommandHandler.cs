// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Gems.Jobs.Quartz.Samples.HappyTreeJobs.FastJob;

[Endpoint("workers/FastJob", "POST", OperationGroup = "workers")]
[JobHandler("FastJob")]
public class FastJobCommandHandler(ILogger<FastJobCommandHandler> logger)
    : IRequestHandler<FastJobCommand>
{
    public Task Handle(FastJobCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fast job done!");
        return Task.CompletedTask;
    }
}
