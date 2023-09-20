// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

using MediatR;

namespace Gems.Data.Tests.Behaviors.Fixtures;

public class SimpleWithInnersAndDifferentTokensCommandHandler : IRequestHandler<SimpleWithInnersAndDifferentTokensCommand>
{
    private readonly IMediator mediator;
    private readonly IUnitOfWorkProvider unitOfWorkProvider;

    public SimpleWithInnersAndDifferentTokensCommandHandler(IMediator mediator, IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.mediator = mediator;
        this.unitOfWorkProvider = unitOfWorkProvider;
    }

    public async Task Handle(SimpleWithInnersAndDifferentTokensCommand request, CancellationToken cancellationToken)
    {
        await this.unitOfWorkProvider.GetUnitOfWork(cancellationToken).CallStoredProcedureAsync($"SimpleWithInnersAndDifferentTokensCommand: {Guid.NewGuid()}")
            .ConfigureAwait(false);
        using (var transactionTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
        {
            await this.mediator.Send(new SimpleCommand(), transactionTokenSource.Token).ConfigureAwait(false);
        }

        using (var transactionTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
        {
            await this.mediator.Send(new SimpleCommand(), transactionTokenSource.Token).ConfigureAwait(false);
        }
    }
}
