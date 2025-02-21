﻿// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Jobs.Quartz.Samples.PersistenceStore.RunExampleWorkerWithData;

[JobHandler("ExampleTriggerWithData")]
[Endpoint("api/v1/ExampleTriggerWithData", "GET", Summary = "Тест", OperationGroup = "Workers")]
public class RunExampleWorkerWithDataCommandHandler : IRequestHandler<RunExampleWorkerCommandWithData>
{
    public Task Handle(RunExampleWorkerCommandWithData request, CancellationToken cancellationToken)
    {
        // do smth
        return Task.Delay(1000, cancellationToken);
    }
}
