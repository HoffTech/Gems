// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

using Microsoft.Extensions.Logging;

namespace Gems.Data.Tests.Behaviors.Fixtures;

public class Transaction
{
    private readonly string unitOfWork;
    private readonly string connection;
    private readonly ILogger<IUnitOfWork> logger;

    public Transaction(string unitOfWork, string connection, ILogger<IUnitOfWork> logger)
    {
        this.unitOfWork = unitOfWork;
        this.connection = connection;
        this.logger = logger;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken).ConfigureAwait(false);
        this.logger.LogTrace($"UnitOfWorkUsing: {this.unitOfWork}, Connection: {this.connection}, Commit transaction: {this.GetHashCode()}");
    }

    public Task DisposeAsync()
    {
        this.logger.LogTrace($"UnitOfWorkUsing: {this.unitOfWork}, Connection: {this.connection}, Dispose transaction: {this.GetHashCode()}");
        return Task.CompletedTask;
    }
}
