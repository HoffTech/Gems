// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

namespace Gems.Patterns.SyncTables
{
    public class EntitiesUpdater
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public EntitiesUpdater(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public Task<TResult> MergeEntitiesAsync<TTargetEntities, TResult>(
            string targetDbKey,
            List<TTargetEntities> entities,
            string functionName,
            string parameterName,
            CancellationToken cancellationToken)
            where TTargetEntities : class
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(targetDbKey, cancellationToken)
                .CallTableFunctionFirstAsync<TResult>(
                    functionName,
                    new Dictionary<string, object> { [parameterName] = entities });
        }
    }
}
