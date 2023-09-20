// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Gems.Data.UnitOfWork;

using Microsoft.Extensions.Logging;

namespace Gems.Data.Tests.Behaviors.Fixtures
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CancellationToken cancellationToken;
        private readonly bool needTransaction;
        private readonly ILogger<IUnitOfWork> logger;
        private Connection connection;
        private Transaction transaction;

        public UnitOfWork(
            bool needTransaction,
            ILogger<IUnitOfWork> logger,
            CancellationToken cancellationToken)
        {
            this.needTransaction = needTransaction;
            this.logger = logger;
            this.cancellationToken = cancellationToken;
        }

        public Task<T> CallScalarFunctionAsync<T>(string functionName, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallScalarFunctionAsync<T>(string functionName, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallScalarFunctionAsync<T>(string functionName, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallScalarFunctionAsync<T>(string functionName, int commandTimeout, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallScalarFunctionAsync<T>(string functionName, int commandTimeout, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallScalarFunctionAsync<T>(string functionName, int commandTimeout, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> CallTableFunctionAsync<T>(string functionName, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> CallTableFunctionAsync<T>(string functionName, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> CallTableFunctionAsync<T>(string functionName, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> CallTableFunctionAsync<T>(string functionName, int commandTimeout, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> CallTableFunctionAsync<T>(string functionName, int commandTimeout, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> CallTableFunctionAsync<T>(string functionName, int commandTimeout, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallTableFunctionFirstAsync<T>(string functionName, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallTableFunctionFirstAsync<T>(string functionName, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallTableFunctionFirstAsync<T>(string functionName, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallTableFunctionFirstAsync<T>(string functionName, int commandTimeout, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallTableFunctionFirstAsync<T>(string functionName, int commandTimeout, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallTableFunctionFirstAsync<T>(string functionName, int commandTimeout, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task CallStoredProcedureAsync(string storeProcedureName, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task CallStoredProcedureAsync(string storeProcedureName, int commandTimeout, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task CallStoredProcedureAsync(string storeProcedureName, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task CallStoredProcedureAsync(string storeProcedureName, int commandTimeout, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public async Task CallStoredProcedureAsync(string storeProcedureName, Enum timeMetricType = null)
        {
            await this.BeginTransactionAsync().ConfigureAwait(false);
            this.logger.LogTrace($"UnitOfWorkUsing: {this.GetHashCode()}, Connection: {this.connection.GetHashCode()}, Call Stored Procedure: {storeProcedureName}");
        }

        public Task CallStoredProcedureAsync(string storeProcedureName, int commandTimeout, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> CallStoredProcedureAsync<T>(string storeProcedureName, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> CallStoredProcedureAsync<T>(string storeProcedureName, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> CallStoredProcedureAsync<T>(string storeProcedureName, int commandTimeout, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> CallStoredProcedureAsync<T>(string storeProcedureName, int commandTimeout, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> CallStoredProcedureAsync<T>(string storeProcedureName, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> CallStoredProcedureAsync<T>(string storeProcedureName, int commandTimeout, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(string storeProcedureName, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(string storeProcedureName, int commandTimeout, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(string storeProcedureName, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(string storeProcedureName, int commandTimeout, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(string storeProcedureName, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(string storeProcedureName, int commandTimeout, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> QueryAsync<T>(string commandText, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> QueryAsync<T>(string commandText, int commandTimeout, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> QueryAsync<T>(string commandText, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> QueryAsync<T>(string commandText, int commandTimeout, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> QueryAsync<T>(string commandText, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> QueryAsync<T>(string commandText, int commandTimeout, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string commandText, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string commandText, int commandTimeout, DynamicParameters parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string commandText, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string commandText, int commandTimeout, Dictionary<string, object> parameters, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string commandText, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string commandText, int commandTimeout, Enum timeMetricType = null)
        {
            throw new NotImplementedException();
        }

        public async Task CommitAsync()
        {
            if (this.transaction == null)
            {
                return;
            }

            await this.transaction.CommitAsync(this.cancellationToken).ConfigureAwait(false);
            await this.transaction.DisposeAsync().ConfigureAwait(false);
            this.transaction = null;
        }

        public async ValueTask DisposeAsync()
        {
            if (this.transaction != null)
            {
                await this.transaction.DisposeAsync().ConfigureAwait(false);
            }

            if (this.connection != null)
            {
                await this.connection.DisposeAsync().ConfigureAwait(false);
            }
        }

        private async Task OpenConnectionAsync()
        {
            if (this.connection != null)
            {
                return;
            }

            this.connection = new Connection(this.GetHashCode().ToString(), this.logger);
            await Task.Delay(1, this.cancellationToken);
        }

        private async Task BeginTransactionAsync()
        {
            await this.OpenConnectionAsync();

            if (!this.needTransaction || this.transaction != null)
            {
                return;
            }

            this.transaction = await this.connection.BeginTransactionAsync(this.cancellationToken).ConfigureAwait(false);
        }
    }
}
