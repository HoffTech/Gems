// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gems.Data.UnitOfWork.EntityFramework;

public class EfUnitOfWork : IEfUnitOfWork
{
    private readonly bool needTransaction;
    private readonly CancellationToken cancellationToken;
    private DbContext dbContext;
    private IDbContextTransaction transaction;

    public EfUnitOfWork(bool needTransaction, Func<CancellationToken, Task<DbContext>> dbContextFactory, CancellationToken cancellationToken)
    {
        this.needTransaction = needTransaction;
        this.cancellationToken = cancellationToken;
        this.DbContextFactory = dbContextFactory;
    }

    internal EfUnitOfWork(bool needTransaction, CancellationToken cancellationToken)
    {
        this.needTransaction = needTransaction;
        this.cancellationToken = cancellationToken;
    }

    internal Func<CancellationToken, Task<DbContext>> DbContextFactory { get; set; }

    public async Task<DbContext> GetDbContextAsync()
    {
        await this.BeginTransactionAsync();
        return this.dbContext;
    }

    public async Task CommitAsync()
    {
        if (this.transaction == null)
        {
            return;
        }

        if (this.dbContext == null)
        {
            return;
        }

        await this.dbContext.SaveChangesAsync(this.cancellationToken).ConfigureAwait(false);
        await this.transaction.CommitAsync(this.cancellationToken).ConfigureAwait(false);
        await this.transaction.DisposeAsync().ConfigureAwait(false);
        this.transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (this.transaction != null)
        {
            await this.transaction.DisposeAsync().ConfigureAwait(false);
            this.transaction = null;
        }

        if (this.dbContext != null)
        {
            await this.dbContext.DisposeAsync().ConfigureAwait(false);
            this.dbContext = null;
        }
    }

    private async Task OpenConnectionAsync()
    {
        if (this.dbContext != null)
        {
            return;
        }

        if (this.DbContextFactory == null)
        {
            throw new InvalidOperationException("Установите фабрику: DbContextFactory");
        }

        this.dbContext = await this.DbContextFactory(this.cancellationToken);
    }

    private async Task BeginTransactionAsync()
    {
        await this.OpenConnectionAsync();

        if (!this.needTransaction || this.transaction != null)
        {
            return;
        }

        this.transaction = await this.dbContext.Database.BeginTransactionAsync(this.cancellationToken).ConfigureAwait(false);
    }
}
