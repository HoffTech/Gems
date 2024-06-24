// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Jobs.Quartz.Samples.PersistenceStore.RunExampleWorker;

[JobHandler("ExampleTrigger")]
[Endpoint("api/v1/ExampleTrigger", "GET", Summary = "Тест", OperationGroup = "Workers")]
public class RunExampleWorkerCommandHandler : IRequestHandler<RunExampleWorkerCommand>
{
    public async Task Handle(RunExampleWorkerCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
    }
}
