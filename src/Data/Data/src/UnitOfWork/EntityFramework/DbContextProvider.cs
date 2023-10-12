// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Gems.Data.UnitOfWork.EntityFramework;

public class DbContextProvider : IDbContextProvider
{
    private readonly IEfUnitOfWorkProvider efUnitOfWorkProvider;

    public DbContextProvider(IEfUnitOfWorkProvider efUnitOfWorkProvider)
    {
        this.efUnitOfWorkProvider = efUnitOfWorkProvider;
    }

    public async Task<TDbContext> GetDbContext<TDbContext>(CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        var unitOfWork = this.efUnitOfWorkProvider.GetUnitOfWork<TDbContext>(cancellationToken);
        return (TDbContext)(await unitOfWork.GetDbContextAsync());
    }
}
