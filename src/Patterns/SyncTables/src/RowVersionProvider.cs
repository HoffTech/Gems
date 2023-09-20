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
    public class RowVersionProvider
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IOptions<ChangeTrackingSyncOptions> options;

        public RowVersionProvider(IUnitOfWorkProvider unitOfWorkProvider, IOptions<ChangeTrackingSyncOptions> options)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.options = options;
        }

        public Task<long> GetLastRowVersionForTableAsync(
            string targetDbKey,
            string tableName,
            CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(targetDbKey, cancellationToken)
                .CallScalarFunctionAsync<long>(
                    this.options.Value.ProviderVersionFunctionInfo.FunctionName,
                    new Dictionary<string, object>
                    {
                        [this.options.Value.ProviderVersionFunctionInfo.TableParameterName] = tableName
                    });
        }
    }
}
