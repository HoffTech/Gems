// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Npgsql;

namespace Gems.Data.Npgsql
{
    /// <summary>
    /// Содержит расширения для работы с Базой Данных PostgreSql через Dapper.
    /// </summary>
    public static class NpgsqlDapperHelper
    {
        [Obsolete("Use CallTableFunctionFirstAsync")]
        public static async Task<T> CallQueryFirstOrDefaultAsync<T>(
            string connectionString,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(
                new CommandDefinition(
                    BuildFunctionSql(functionName, parameters),
                    parameters,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        [Obsolete("Use CallScalarFunctionAsync")]
        public static async Task<T> CallExecuteFunctionAsync<T>(
            string connectionString,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<T>(
                new CommandDefinition(
                    BuildFunctionSql(functionName, parameters),
                    parameters,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        [Obsolete("Use CallScalarFunctionAsync")]
        public static async Task<T> CallExecuteFunctionAsync<T>(
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
            string functionName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return await connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName, parameters),
                parameters,
                transaction,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static Task<T> CallScalarFunctionAsync<T>(
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName),
                null,
                null,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                BuildTableFunctionSql(functionName),
                null,
                null,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<NpgsqlConnection> mappingConfig,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<NpgsqlConnection> mappingConfig,
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            Action<NpgsqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            Action<NpgsqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<NpgsqlConnection> mappingConfig,
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
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<NpgsqlConnection> mappingConfig,
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
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName),
                        null,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<NpgsqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
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
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        BuildStoredProcedureSql(storeProcedureName, parameters),
                        parameters,
                        null,
                        null,
                        CommandType.Text,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<NpgsqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
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
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<NpgsqlConnection> mappingConfig,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<NpgsqlConnection> mappingConfig,
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            Action<NpgsqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            Action<NpgsqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
            NpgsqlTransaction transaction,
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
            NpgsqlConnection connection,
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
            NpgsqlConnection connection,
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
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
            await using var connection = new NpgsqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                    commandText,
                    null,
                    null,
                    null,
                    CommandType.Text,
                    cancellationToken: cancellationToken)).ConfigureAwait(false);
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
            return $"call {functionName}() ";
        }

        private static string BuildStoredProcedureSql(string functionName, DynamicParameters parameters)
        {
            return $"call {functionName}({string.Join(",", parameters.ParameterNames.Select(p => $"@{p}"))}) ";
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
                    AddDynamicParameter(dynamicParameters, parameter);
                    continue;
                }

                var t = parameter.Value.GetType();
                if (t.IsValueType || t == typeof(string) || t == typeof(Guid))
                {
                    AddDynamicParameter(dynamicParameters, parameter);
                }
                else
                {
                    AddDynamicParameter(dynamicParameters, parameter, DbType.Object);
                }
            }

            return dynamicParameters;
        }

        private static void AddDynamicParameter(DynamicParameters dynamicParameters, KeyValuePair<string, object> parameter)
        {
            var keyParts = parameter.Key.Split(":");
            if (keyParts.Length == 2)
            {
                dynamicParameters.Add(keyParts[0], parameter.Value, direction: MapToDirection(keyParts[1]));
            }
            else
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }
        }

        private static void AddDynamicParameter(DynamicParameters dynamicParameters, KeyValuePair<string, object> parameter, DbType dbType)
        {
            var keyParts = parameter.Key.Split(":");
            if (keyParts.Length == 2)
            {
                dynamicParameters.Add(keyParts[0], parameter.Value, dbType, MapToDirection(keyParts[1]));
            }
            else
            {
                dynamicParameters.Add(parameter.Key, parameter.Value, dbType);
            }
        }

        private static ParameterDirection? MapToDirection(string direction)
        {
            return direction.ToUpper() switch
            {
                "IN" => ParameterDirection.Input,
                "OUT" => ParameterDirection.Output,
                "INOUT" => ParameterDirection.InputOutput,
                "RETURN" => ParameterDirection.ReturnValue,
                _ => ParameterDirection.Input
            };
        }
    }
}
