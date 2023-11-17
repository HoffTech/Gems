// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

namespace Gems.Patterns.SyncTables
{
    public class ExternalEntitiesProvider
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public ExternalEntitiesProvider(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public Task<List<TExternalEntity>> GetExternalEntitiesByQueryWithVersionAsync<TExternalEntity>(
            string sourceDbKey,
            long version,
            string query,
            int commandTimeout,
            Enum dbQueryMetricType,
            CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(sourceDbKey, cancellationToken)
                .QueryAsync<TExternalEntity>(
                    query,
                    commandTimeout,
                    new Dictionary<string, object> { ["@version"] = version },
                    dbQueryMetricType);
        }

        public Task<List<TExternalEntity>> GetExternalEntitiesByQueryAsync<TExternalEntity>(
            string sourceDbKey,
            string query,
            int commandTimeout,
            Enum dbQueryMetricType,
            CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(sourceDbKey, cancellationToken)
                .QueryAsync<TExternalEntity>(query, commandTimeout, dbQueryMetricType);
        }
    }
}
