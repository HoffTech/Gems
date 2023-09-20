// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

using MediatR;

namespace Gems.Data.Tests.Behaviors.Fixtures;

public class SimpleWithUnitOfWorkAndInnerUnitOfWorksAndDifferentTokensCommandHandler : IRequestHandler<SimpleWithUnitOfWorkAndInnerUnitOfWorksAndDifferentTokensCommand>
{
    private readonly IMediator mediator;
    private readonly IUnitOfWorkProvider unitOfWorkProvider;

    public SimpleWithUnitOfWorkAndInnerUnitOfWorksAndDifferentTokensCommandHandler(IMediator mediator, IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.mediator = mediator;
        this.unitOfWorkProvider = unitOfWorkProvider;
    }

    public async Task Handle(SimpleWithUnitOfWorkAndInnerUnitOfWorksAndDifferentTokensCommand request, CancellationToken cancellationToken)
    {
        await this.unitOfWorkProvider.GetUnitOfWork(cancellationToken).CallStoredProcedureAsync($"SimpleWithUnitOfWorkAndInnerUnitOfWorksAndDifferentTokensCommand: {Guid.NewGuid()}")
            .ConfigureAwait(false);
        using (var transactionTokenSource1 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
        {
            await this.mediator.Send(new SimpleWithUnitOfWorkCommand(), transactionTokenSource1.Token).ConfigureAwait(false);
        }

        using (var transactionTokenSource2 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
        {
            await this.mediator.Send(new SimpleWithUnitOfWorkCommand(), transactionTokenSource2.Token).ConfigureAwait(false);
        }
    }
}
