// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;
using Gems.Tasks;

using MediatR;

namespace Gems.Data.Tests.Behaviors.Fixtures;

public class SimpleWithUnitOfWorkAndInnerUnitOfWorksInsideAsyncAwaiterAndDifferentTokensCommandHandler : IRequestHandler<SimpleWithUnitOfWorkAndInnerUnitOfWorksInsideAsyncAwaiterAndDifferentTokensCommand>
{
    private readonly IMediator mediator;
    private readonly IUnitOfWorkProvider unitOfWorkProvider;

    public SimpleWithUnitOfWorkAndInnerUnitOfWorksInsideAsyncAwaiterAndDifferentTokensCommandHandler(IMediator mediator, IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.mediator = mediator;
        this.unitOfWorkProvider = unitOfWorkProvider;
    }

    public async Task Handle(SimpleWithUnitOfWorkAndInnerUnitOfWorksInsideAsyncAwaiterAndDifferentTokensCommand request, CancellationToken cancellationToken)
    {
        await this.unitOfWorkProvider.GetUnitOfWork(cancellationToken).CallStoredProcedureAsync($"SimpleWithUnitOfWorkAndInnerUnitOfWorksInsideAsyncAwaiterAndDifferentTokensCommand: {Guid.NewGuid()}")
            .ConfigureAwait(false);
        var token1 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken).Token;
        var token2 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken).Token;
        var task1 = AsyncAwaiter.AwaitAsync(
            nameof(SimpleWithUnitOfWorkCommand),
            () => this.mediator.Send(new SimpleWithUnitOfWorkCommand(), token1),
            2);
        var task2 = AsyncAwaiter.AwaitAsync(
            nameof(SimpleWithUnitOfWorkCommand),
            () => this.mediator.Send(new SimpleWithUnitOfWorkCommand(), token2),
            2);

        await Task.WhenAll(task1, task2);
    }
}
