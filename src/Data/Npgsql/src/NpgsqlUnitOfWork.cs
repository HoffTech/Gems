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

using Npgsql;

namespace Gems.Data.Npgsql
{
    public class NpgsqlUnitOfWork : IUnitOfWork
    {
        private readonly CancellationToken cancellationToken;
        private readonly IConnectionStringProvider connectionStringProvider;
        private readonly bool needTransaction;
        private readonly TimeMetricProvider timeMetricProvider;
        private NpgsqlConnection connection;
        private NpgsqlTransaction transaction;

        public NpgsqlUnitOfWork(
            IConnectionStringProvider connectionStringProvider,
            bool needTransaction,
            TimeMetricProvider timeMetricProvider,
            CancellationToken cancellationToken)
        {
            this.connectionStringProvider = connectionStringProvider;
            this.needTransaction = needTransaction;
            this.timeMetricProvider = timeMetricProvider;
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
                return await NpgsqlDapperHelper.CallScalarFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallScalarFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallScalarFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallScalarFunctionAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallScalarFunctionAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallScalarFunctionAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionFirstAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionFirstAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionFirstAsync<T>(
                    this.connection,
                    this.transaction,
                    functionName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionFirstAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionFirstAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallTableFunctionFirstAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                await NpgsqlDapperHelper.CallStoredProcedureAsync(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                await NpgsqlDapperHelper.CallStoredProcedureAsync(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                await NpgsqlDapperHelper.CallStoredProcedureAsync(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                await NpgsqlDapperHelper.CallStoredProcedureAsync(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                await NpgsqlDapperHelper.CallStoredProcedureAsync(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    storeProcedureName,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryFirstOrDefaultAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    parameters,
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryFirstOrDefaultAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryFirstOrDefaultAsync<T>(
                    this.connection,
                    this.transaction,
                    commandText,
                    new DynamicParameters(),
                    this.cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
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
                return await NpgsqlDapperHelper.QueryFirstOrDefaultAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
            }
        }

        public async Task CallStoredProcedureAsync(
            string storeProcedureName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                await NpgsqlDapperHelper.CallStoredProcedureAsync(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
            }
        }

        public async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                return await NpgsqlDapperHelper.CallStoredProcedureFirstOrDefaultAsync<T>(
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
                await this.CloseConnectionAsync().ConfigureAwait(false);
            }
        }

        public async IAsyncEnumerable<T> ExecuteReaderAsync<T>(
            string commandText,
            Enum timeMetricType = null)
        {
            var timeMetric = this.timeMetricProvider.GetTimeMetric(timeMetricType, null);
            try
            {
                await this.BeginTransactionAsync().ConfigureAwait(false);
                await foreach (var item in NpgsqlDapperHelper
                                   .ExecuteReaderAsync<T>(this.connection, commandText, this.cancellationToken)
                                   .ConfigureAwait(false))
                {
                    yield return item;
                }
            }
            finally
            {
                await timeMetric.DisposeMetric().ConfigureAwait(false);
                await this.CloseConnectionAsync().ConfigureAwait(false);
            }
        }

        public async Task CommitAsync()
        {
            if (this.transaction == null)
            {
                await this.CloseConnectionAsync().ConfigureAwait(false);
                return;
            }

            await this.transaction.CommitAsync(this.cancellationToken).ConfigureAwait(false);
            await this.EndTransactionAsync().ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await this.EndTransactionAsync().ConfigureAwait(false);
        }

        private async Task OpenConnectionAsync()
        {
            if (this.connection != null)
            {
                return;
            }

            this.connection = new NpgsqlConnection(this.connectionStringProvider.Value);
            await this.connection.OpenAsync(this.cancellationToken).ConfigureAwait(false);
        }

        private async Task CloseConnectionAsync()
        {
            if (this.transaction != null)
            {
                return;
            }

            if (this.connection != null)
            {
                await this.connection.DisposeAsync().ConfigureAwait(false);
                this.connection = null;
            }
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

        private async Task EndTransactionAsync()
        {
            if (this.transaction != null)
            {
                await this.transaction.DisposeAsync().ConfigureAwait(false);
                this.transaction = null;
            }

            await this.CloseConnectionAsync().ConfigureAwait(false);
        }
    }
}
