// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using MediatR;

namespace Gems.Jobs.Quartz;

public interface IQuartzEnqueueManager
{
    Task Enqueue<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, IRequest, new();
}
