// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Repository
{
    public class DestinationEntitiesUpdater
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public DestinationEntitiesUpdater(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public Task<TResult> MergeEntitiesAsync<TTargetEntities, TResult>(
            string targetDbKey,
            List<TTargetEntities> entities,
            bool enableFullChangesLog,
            string functionName,
            string parameterName,
            int commandTimeout,
            CancellationToken cancellationToken)
            where TTargetEntities : class
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(targetDbKey, cancellationToken)
                .CallTableFunctionFirstAsync<TResult>(
                    functionName,
                    commandTimeout: commandTimeout,
                    new Dictionary<string, object>
                    {
                        [parameterName] = entities,
                        ["enable_full_changes_log"] = enableFullChangesLog
                    });
        }

        public Task ClearDestinationAsync(
            string targetDbKey,
            string functionName,
            CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(targetDbKey, cancellationToken)
                .CallStoredProcedureAsync(functionName);
        }
    }
}
