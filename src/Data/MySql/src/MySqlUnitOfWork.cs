// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Gems.Data.UnitOfWork;
using Gems.Metrics;
using Gems.Metrics.Data;

using MySqlConnector;

namespace Gems.Data.MySql
{
    public class MySqlUnitOfWork : IUnitOfWork
    {
        private readonly CancellationToken cancellationToken;
        private readonly IConnectionStringProvider connectionStringProvider;
        private readonly bool needTransaction;
        private readonly TimeMetricProvider timeMetricProvider;
        private readonly SshClientOptions sshClientOptions;
        private MySqlConnection connection;
        private MySqlTransaction transaction;

        public MySqlUnitOfWork(
            IConnectionStringProvider connectionStringProvider,
            bool needTransaction,
            TimeMetricProvider timeMetricProvider,
            SshClientOptions sshClientOptions,
            CancellationToken cancellationToken)
        {
            this.connectionStringProvider = connectionStringProvider;
            this.needTransaction = needTransaction;
            this.timeMetricProvider = timeMetricProvider;
            this.sshClientOptions = sshClientOptions;
            this.cancellationToken = cancellationToken;
        }

        public async Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallScalarFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallScalarFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallScalarFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallScalarFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallScalarFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallScalarFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    new DynamicParameters(),
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    new DynamicParameters(),
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionFirstAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionFirstAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionFirstAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionFirstAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionFirstAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            int commandTimeout,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, functionName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallTableFunctionFirstAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    new DynamicParameters(),
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task CallStoredProcedureAsync(
            string storeProcedureName,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                await MySqlDapperHelper.CallStoredProcedureAsync(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task CallStoredProcedureAsync(
            string storeProcedureName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                await MySqlDapperHelper.CallStoredProcedureAsync(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task CallStoredProcedureAsync(
            string storeProcedureName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                await MySqlDapperHelper.CallStoredProcedureAsync(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task CallStoredProcedureAsync(
            string storeProcedureName,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                await MySqlDapperHelper.CallStoredProcedureAsync(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task CallStoredProcedureAsync(
            string storeProcedureName,
            int commandTimeout,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                await MySqlDapperHelper.CallStoredProcedureAsync(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    new DynamicParameters(),
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    new DynamicParameters(),
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, storeProcedureName);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    new DynamicParameters(),
                    commandTimeout,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> QueryAsync<T>(
            string commandText,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> QueryAsync<T>(
            string commandText,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryAsync<T>(
                           this.connection,
                           this.transaction,
                           commandText,
                           parameters,
                           commandTimeout,
                           this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> QueryAsync<T>(
            string commandText,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> QueryAsync<T>(
            string commandText,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryAsync<T>(
                           this.connection,
                           this.transaction,
                           commandText,
                           parameters,
                           commandTimeout,
                           this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> QueryAsync<T>(
            string commandText,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<List<T>> QueryAsync<T>(
            string commandText,
            int commandTimeout,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryAsync<T>(
                           this.connection,
                           this.transaction,
                           commandText,
                           new DynamicParameters(),
                           commandTimeout,
                           this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryFirstOrDefaultAsync<T>(
                           this.connection,
                           this.transaction,
                           commandText,
                           parameters,
                           commandTimeout,
                           this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryFirstOrDefaultAsync<T>(
                           this.connection,
                           this.transaction,
                           commandText,
                           parameters,
                           commandTimeout,
                           this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            int commandTimeout,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await MySqlDapperHelper.QueryFirstOrDefaultAsync<T>(
                           this.connection,
                           this.transaction,
                           commandText,
                           new DynamicParameters(),
                           commandTimeout,
                           this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
            }
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
                this.transaction = null;
            }

            if (this.connection != null)
            {
                await this.connection.DisposeAsync().ConfigureAwait(false);
                this.connection = null;
            }
        }

        public async Task CallStoredProcedureAsync(
            string storeProcedureName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            await this.BeginTransactionAsync().ConfigureAwait(false);
            await MySqlDapperHelper.CallStoredProcedureAsync(
                this.connection,
                this.transaction,
                storeProcedureName,
                new DynamicParameters(),
                commandTimeout,
                this.cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            await this.BeginTransactionAsync().ConfigureAwait(false);
            return await MySqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
                this.connection,
                this.transaction,
                storeProcedureName,
                parameters,
                commandTimeout,
                this.cancellationToken).ConfigureAwait(false);
        }

        private async Task OpenConnectionAsync()
        {
            SshAgent.OpenSsh(this.sshClientOptions);

            if (this.connection != null)
            {
                return;
            }

            this.connection = new MySqlConnection(this.connectionStringProvider.Value);
            await this.connection.OpenAsync(this.cancellationToken).ConfigureAwait(false);
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
