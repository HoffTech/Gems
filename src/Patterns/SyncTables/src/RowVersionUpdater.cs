// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;
using Gems.Patterns.SyncTables.Options;

using Microsoft.Extensions.Options;

namespace Gems.Patterns.SyncTables
{
    public class RowVersionUpdater
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IOptions<ChangeTrackingSyncOptions> options;

        public RowVersionUpdater(IUnitOfWorkProvider unitOfWorkProvider, IOptions<ChangeTrackingSyncOptions> options)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.options = options;
        }

        public Task UpsertRowVersionByTableNameAsync(
            string targetDbKey,
            string tableName,
            long rowVersion,
            CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(targetDbKey, cancellationToken)
                .CallStoredProcedureAsync(
                    this.options.Value.UpsertVersionFunctionInfo.FunctionName,
                    new Dictionary<string, object>
                    {
                        [this.options.Value.UpsertVersionFunctionInfo.TableParameterName] = tableName,
                        [this.options.Value.UpsertVersionFunctionInfo.RowVersionParameterName] = rowVersion
                    });
        }
    }
}
