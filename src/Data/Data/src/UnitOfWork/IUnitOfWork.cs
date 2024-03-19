// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dapper;

namespace Gems.Data.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task CommitAsync();

        Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            Enum timeMetricType = null);

        Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<T> CallScalarFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            Enum timeMetricType = null);

        Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            Enum timeMetricType = null);

        Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<List<T>> CallTableFunctionAsync<T>(
            string functionName,
            int commandTimeout,
            Enum timeMetricType = null);

        Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            Enum timeMetricType = null);

        Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<T> CallTableFunctionFirstAsync<T>(
            string functionName,
            int commandTimeout,
            Enum timeMetricType = null);

        Task CallStoredProcedureAsync(
            string storeProcedureName,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task CallStoredProcedureAsync(
            string storeProcedureName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task CallStoredProcedureAsync(
            string storeProcedureName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task CallStoredProcedureAsync(
            string storeProcedureName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task CallStoredProcedureAsync(
            string storeProcedureName,
            Enum timeMetricType = null);

        Task CallStoredProcedureAsync(
            string storeProcedureName,
            int commandTimeout,
            Enum timeMetricType = null);

        Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            Enum timeMetricType = null);

        Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            Enum timeMetricType = null);

        Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            Enum timeMetricType = null);

        Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string storeProcedureName,
            int commandTimeout,
            Enum timeMetricType = null);

        Task<List<T>> QueryAsync<T>(
            string commandText,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<List<T>> QueryAsync<T>(
            string commandText,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<List<T>> QueryAsync<T>(
            string commandText,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<List<T>> QueryAsync<T>(
            string commandText,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<List<T>> QueryAsync<T>(
            string commandText,
            Enum timeMetricType = null);

        Task<List<T>> QueryAsync<T>(
            string commandText,
            int commandTimeout,
            Enum timeMetricType = null);

        Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            int commandTimeout,
            DynamicParameters parameters,
            Enum timeMetricType = null);

        Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            int commandTimeout,
            Dictionary<string, object> parameters,
            Enum timeMetricType = null);

        Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            Enum timeMetricType = null);

        Task<T> QueryFirstOrDefaultAsync<T>(
            string commandText,
            int commandTimeout,
            Enum timeMetricType = null);

        IAsyncEnumerable<T> ExecuteReaderAsync<T>(
            string commandText,
            Enum timeMetricType = null);
    }
}
