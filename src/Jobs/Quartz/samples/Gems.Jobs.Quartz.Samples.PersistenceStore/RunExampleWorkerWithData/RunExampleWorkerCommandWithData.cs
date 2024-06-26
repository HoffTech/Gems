﻿// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

namespace Gems.Jobs.Quartz.Samples.PersistenceStore.RunExampleWorkerWithData;

public class RunExampleWorkerCommandWithData : IRequest
{
    public string SomeData { get; init; }

    public int SomeData2 { get; init; }
}
