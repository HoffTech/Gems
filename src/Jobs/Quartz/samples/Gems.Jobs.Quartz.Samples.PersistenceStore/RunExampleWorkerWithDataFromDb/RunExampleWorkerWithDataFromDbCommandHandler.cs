// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Jobs.Quartz.Samples.PersistenceStore.RunExampleWorkerWithDataFromDb;

[JobHandler("ExampleTriggerFromDb")]
[Endpoint("api/v1/ExampleTriggerFromDb", "GET", Summary = "Тест", OperationGroup = "Workers")]
public class RunExampleWorkerWithDataFromDbCommandHandler : IRequestHandler<RunExampleWorkerWithDataFromDbCommand>
{
    public async Task Handle(RunExampleWorkerWithDataFromDbCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
    }
}
