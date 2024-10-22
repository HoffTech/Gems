// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Gems.Jobs.Quartz.Samples.HappyTreeJobs.UnstableJob;

[Endpoint("workers/UnstableJob", "POST", OperationGroup = "workers")]
[JobHandler("UnstableJob")]
public class UnstableJobCommandHandler(ILogger<UnstableJobCommandHandler> logger)
    : IRequestHandler<UnstableJobCommand>
{
    public async Task Handle(UnstableJobCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unstable job start!");
        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

        if (new Random().NextDouble() > 0.9)
        {
            throw new Exception("noluck");
        }

        logger.LogInformation("Unstable job done!");
    }
}
