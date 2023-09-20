// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

using Microsoft.Extensions.Logging;

namespace Gems.Data.Tests.Behaviors.Fixtures;

public class Connection
{
    private readonly string unitOfWork;
    private readonly ILogger<IUnitOfWork> logger;

    public Connection(string unitOfWork, ILogger<IUnitOfWork> logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Transaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
        var transaction = new Transaction(this.unitOfWork, this.GetHashCode().ToString(), this.logger);
        this.logger.LogTrace($"UnitOfWorkUsing: {this.unitOfWork}, Connection: {this.GetHashCode()}, Begin Transaction: {transaction.GetHashCode()}");
        return transaction;
    }

    public Task DisposeAsync()
    {
        this.logger.LogTrace($"UnitOfWorkUsing: {this.unitOfWork}, Connection: {this.GetHashCode()}, Dispose Connection");
        return Task.CompletedTask;
    }
}
