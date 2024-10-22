// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Gems.Jobs.Quartz.Samples.HappyTreeJobs.LongJob;

[Endpoint("workers/LongJob", "POST", OperationGroup = "workers")]
[JobHandler("LongJob")]
public class LongJobCommandHandler(ILogger<LongJobCommandHandler> logger)
    : IRequestHandler<LongJobCommand>
{
    public async Task Handle(LongJobCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Long job start!");
        await Task.Delay(TimeSpan.FromSeconds(300), cancellationToken);
        logger.LogInformation("Long job done!");
    }
}
