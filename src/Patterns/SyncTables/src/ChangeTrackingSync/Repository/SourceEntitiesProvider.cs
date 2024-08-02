// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Repository;

public class SourceEntitiesProvider
{
    private readonly IUnitOfWorkProvider unitOfWorkProvider;

    public SourceEntitiesProvider(IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.unitOfWorkProvider = unitOfWorkProvider;
    }

    public Task<List<TSourceEntity>> LoadChangesFromVersion<TSourceEntity>(
        string sourceDbKey,
        long version,
        string query,
        int commandTimeout,
        int bathcSize,
        CancellationToken cancellationToken)
    {
        return this.unitOfWorkProvider
            .GetUnitOfWork(sourceDbKey, cancellationToken)
            .QueryAsync<TSourceEntity>(
                query,
                commandTimeout,
                new Dictionary<string, object>
                {
                    ["@version"] = version,
                    ["@batchSize"] = bathcSize
                });
    }

    public Task<List<TSourceEntity>> LoadFull<TSourceEntity>(
        string sourceDbKey,
        string query,
        int commandTimeout,
        long offset,
        int bathcSize,
        CancellationToken cancellationToken)
    {
        return this.unitOfWorkProvider
            .GetUnitOfWork(sourceDbKey, cancellationToken)
            .QueryAsync<TSourceEntity>(
                query,
                commandTimeout,
                new Dictionary<string, object>
                {
                    ["@batchSize"] = bathcSize,
                    ["@offset"] = offset
                });
    }
}
