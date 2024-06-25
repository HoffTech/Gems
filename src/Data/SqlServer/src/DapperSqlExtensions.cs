// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

namespace Gems.Data.SqlServer
{
    public static class SqlDapperHelper
    {
        public static async Task<List<T>> GetAllAsync<T>(
            string connectionString,
            TableDescription description,
            int commandTimeout = 15,
            CancellationToken cancellationToken = default) where T : class
        {
            await using var connection = new SqlConnection(connectionString);
            var query = $"SELECT {string.Join(",", description.Fields)} FROM {description.TableName}";
            var results = await connection.QueryAsync<T>(
                new CommandDefinition(query, new { }, commandTimeout: commandTimeout, cancellationToken: cancellationToken)).ConfigureAwait(false);
            return results.ToList();
        }

        public static async Task<Dictionary<TKey, T>> GetDictionaryAsync<T, TKey>(
            string connectionString,
            TableDescription description,
            Func<T, TKey> keySelector,
            int commandTimeout = 15,
            CancellationToken cancellationToken = default) where T : class
        {
            await using var connection = new SqlConnection(connectionString);
            var query = $"SELECT {string.Join(",", description.Fields)} FROM {description.TableName}";
            var results = await connection.QueryAsync<T>(
                new CommandDefinition(query, new { }, commandTimeout: commandTimeout, cancellationToken: cancellationToken)).ConfigureAwait(false);
            return results.ToDictionary(keySelector);
        }

        [Obsolete("Use QueryAsync")]
        public static async Task<List<T>> GetBySqlAsync<T>(
            string connectionString,
            string sql,
            DynamicParameters parameters,
            int commandTimeout = 15,
            CancellationToken cancellationToken = default) where T : class
        {
            await using var connection = new SqlConnection(connectionString);
            var results = await connection.QueryAsync<T>(new CommandDefinition(sql, parameters, commandTimeout: commandTimeout, cancellationToken: cancellationToken))
                .ConfigureAwait(false);
            return results.ToList();
        }

        [Obsolete("Use QueryFirstOrDefaultAsync")]
        public static async Task<T> GetSingeOrDefaultAsync<T>(
            string connectionString,
            string sql,
            DynamicParameters parameters,
            int commandTimeout = 15,
            CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(connectionString);
            return (await connection.QueryAsync<T>(
                new CommandDefinition(sql, parameters, commandTimeout: commandTimeout, cancellationToken: cancellationToken))
                .ConfigureAwait(false)).SingleOrDefault();
        }

        public static Task<T> CallScalarFunctionAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            return connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName, parameters),
                parameters,
                transaction,
                commandTimeout,
                CommandType.Text,
                cancellationToken: cancellationToken));
        }

        public static Task<T> CallScalarFunctionAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName, parameters),
                parameters,
                transaction,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken));
        }

        public static Task<T> CallScalarFunctionAsync<T>(
            SqlConnection connection,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName, parameters),
                parameters,
                null,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken));
        }

        public static Task<T> CallScalarFunctionAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName, parameters),
                parameters,
                transaction,
                commandTimeout,
                CommandType.Text,
                cancellationToken: cancellationToken));
        }

        public static Task<T> CallScalarFunctionAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName, parameters),
                parameters,
                transaction,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken));
        }

        public static Task<T> CallScalarFunctionAsync<T>(
            SqlConnection connection,
            string functionName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName, parameters),
                parameters,
                null,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken));
        }

        public static Task<T> CallScalarFunctionAsync<T>(
            SqlConnection connection,
            string functionName,
            CancellationToken cancellationToken)
        {
            return connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName),
                null,
                null,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken));
        }

        public static async Task<T> CallScalarFunctionAsync<T>(
            string connectionString,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName, parameters),
                parameters,
                null,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallScalarFunctionAsync<T>(
            string connectionString,
            string functionName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName, parameters),
                parameters,
                null,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallScalarFunctionAsync<T>(
            string connectionString,
            string functionName,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName),
                null,
                null,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            return (await connection.QueryAsync<T>(new CommandDefinition(
                BuildTableFunctionSql(functionName, parameters),
                parameters,
                transaction,
                commandTimeout,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            SqlConnection connection,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    transaction,
                    commandTimeout,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            SqlConnection connection,
            string functionName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            SqlConnection connection,
            string functionName,
            CancellationToken cancellationToken)
        {
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName),
                    null,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            string connectionString,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            string connectionString,
            string functionName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            string connectionString,
            string functionName,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName),
                    null,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<T> CallTableFunctionFirstAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                BuildTableFunctionSql(functionName, parameters),
                parameters,
                transaction,
                commandTimeout,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallTableFunctionFirstAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallTableFunctionFirstAsync<T>(
            SqlConnection connection,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallTableFunctionFirstAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    transaction,
                    commandTimeout,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallTableFunctionFirstAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string functionName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallTableFunctionFirstAsync<T>(
            SqlConnection connection,
            string functionName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallTableFunctionFirstAsync<T>(
            SqlConnection connection,
            string functionName,
            CancellationToken cancellationToken)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName),
                    null,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallTableFunctionFirstAsync<T>(
            string connectionString,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallTableFunctionFirstAsync<T>(
            string connectionString,
            string functionName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName, parameters),
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> CallTableFunctionFirstAsync<T>(
            string connectionString,
            string functionName,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    BuildTableFunctionSql(functionName),
                    null,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    BuildStoredProcedureSql(storeProcedureName, parameters),
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            SqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            SqlConnection connection,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName),
                        null,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            SqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                new CommandDefinition(
                    BuildStoredProcedureSql(storeProcedureName, parameters),
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            SqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            SqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            string connectionString,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName),
                        null,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            return connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return connection.QueryAsync<T>(
                new CommandDefinition(
                    BuildStoredProcedureSql(storeProcedureName, parameters),
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken));
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            SqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            SqlConnection connection,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            return connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName),
                        null,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            SqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryAsync<T>(
                new CommandDefinition(
                    BuildStoredProcedureSql(storeProcedureName, parameters),
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken));
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            SqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            SqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName),
                        null,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        transaction,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            SqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            SqlConnection connection,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName),
                        null,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            SqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        transaction,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            SqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken));
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            SqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string connectionString,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName),
                        null,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<SqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<List<T>> QueryAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string commandText,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            return (await connection.QueryAsync<T>(new CommandDefinition(
                commandText,
                parameters,
                transaction,
                commandTimeout,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string commandText,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            SqlConnection connection,
            string commandText,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string commandText,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    transaction,
                    commandTimeout,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string commandText,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            SqlConnection connection,
            string commandText,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            SqlConnection connection,
            string commandText,
            CancellationToken cancellationToken)
        {
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    null,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            string connectionString,
            string commandText,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    null,
                    commandTimeout,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            string connectionString,
            string commandText,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            string connectionString,
            string commandText,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    null,
                    commandTimeout,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            string connectionString,
            string commandText,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<List<T>> QueryAsync<T>(
            string connectionString,
            string commandText,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return (await connection.QueryAsync<T>(new CommandDefinition(
                    commandText,
                    null,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false))
                .ToList();
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string commandText,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                commandText,
                parameters,
                transaction,
                commandTimeout,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string commandText,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            SqlConnection connection,
            string commandText,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string commandText,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    transaction,
                    commandTimeout,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            SqlConnection connection,
            DbTransaction transaction,
            string commandText,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    transaction,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            SqlConnection connection,
            string commandText,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            SqlConnection connection,
            string commandText,
            CancellationToken cancellationToken)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    commandText,
                    null,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            string connectionString,
            string commandText,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            string connectionString,
            string commandText,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    commandText,
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<T> QueryFirstOrDefaultAsync<T>(
            string connectionString,
            string commandText,
            CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    commandText,
                    null,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async IAsyncEnumerable<T> ExecuteReaderAsync<T>(
            SqlConnection connection,
            string commandText,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            if (reader.HasRows)
            {
                yield break;
            }

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return reader.GetRowParser<T>()(reader);
            }
        }

        private static string BuildFunctionSql(string functionName)
        {
            return $"select {functionName}() ";
        }

        private static string BuildFunctionSql(string functionName, DynamicParameters parameters)
        {
            return $"select {functionName}({string.Join(",", parameters.ParameterNames.Select(p => $"@{p}"))}) ";
        }

        private static string BuildTableFunctionSql(string functionName)
        {
            return $"select * from {functionName}() ";
        }

        private static string BuildTableFunctionSql(string functionName, DynamicParameters parameters)
        {
            return $"select * from {functionName}({string.Join(",", parameters.ParameterNames.Select(p => $"@{p}"))}) ";
        }

        private static string BuildStoredProcedureSql(string functionName)
        {
            return $"exec {functionName};";
        }

        private static string BuildStoredProcedureSql(string functionName, DynamicParameters parameters)
        {
            return $"exec {functionName} {string.Join(", ", parameters.ParameterNames.Select(p => $"@{p}"))};";
        }

        private static DynamicParameters MapToDynamicParameters(Dictionary<string, object> parameters)
        {
            var dynamicParameters = new DynamicParameters();
            if (parameters == null)
            {
                return dynamicParameters;
            }

            foreach (var parameter in parameters)
            {
                if (parameter.Value == null)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                    continue;
                }

                var t = parameter.Value.GetType();
                if (t.IsValueType || t == typeof(string) || t == typeof(Guid))
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }
                else
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value, DbType.Object);
                }
            }

            return dynamicParameters;
        }
    }
}
