// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

using MediatR;

namespace Gems.Data.Tests.Behaviors.Fixtures;

public class SimpleWithUnitOfWorkCommandHandler : IRequestHandler<SimpleWithUnitOfWorkCommand>
{
    private readonly IUnitOfWorkProvider unitOfWorkProvider;

    public SimpleWithUnitOfWorkCommandHandler(IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.unitOfWorkProvider = unitOfWorkProvider;
    }

    public Task Handle(SimpleWithUnitOfWorkCommand request, CancellationToken cancellationToken)
    {
        return this.unitOfWorkProvider.GetUnitOfWork(cancellationToken).CallStoredProcedureAsync($"SimpleWithUnitOfWorkCommand: {Guid.NewGuid()}");
    }
}
