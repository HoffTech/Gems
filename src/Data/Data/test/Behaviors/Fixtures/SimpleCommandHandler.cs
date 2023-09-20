// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

using MediatR;

namespace Gems.Data.Tests.Behaviors.Fixtures;

public class SimpleCommandHandler : IRequestHandler<SimpleCommand>
{
    private readonly IUnitOfWorkProvider unitOfWorkProvider;

    public SimpleCommandHandler(IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.unitOfWorkProvider = unitOfWorkProvider;
    }

    public async Task Handle(SimpleCommand request, CancellationToken cancellationToken)
    {
        await this.unitOfWorkProvider.GetUnitOfWork(cancellationToken).CallStoredProcedureAsync($"SimpleCommand: {Guid.NewGuid()}")
            .ConfigureAwait(false);
    }
}
