// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

namespace Gems.Jobs.Quartz.Samples.PersistenceStore.RunExampleWorkerWithDataFromDb;

public class RunExampleWorkerWithDataFromDbCommand : IRequest
{
    public string Id { get; init; }

    public int Data { get; init; }
}
