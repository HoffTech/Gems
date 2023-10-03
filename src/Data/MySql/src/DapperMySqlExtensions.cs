// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using MySqlConnector;

namespace Gems.Data.MySql
{
    /// <summary>
    /// Содержит расширения для работы с Базой Данных MySql через Dapper.
    /// </summary>
    public static class MySqlDapperHelper
    {
        public static Task<T> CallScalarFunctionAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
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
            MySqlConnection connection,
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<T>(new CommandDefinition(
                BuildFunctionSql(functionName),
                null,
                null,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task<List<T>> CallTableFunctionAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
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
            MySqlConnection connection,
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
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
            MySqlConnection connection,
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
                BuildTableFunctionSql(functionName),
                null,
                null,
                null,
                CommandType.Text,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    storeProcedureName,
                    parameters,
                    transaction,
                    null,
                    CommandType.StoredProcedure,
                    cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            MySqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            MySqlConnection connection,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        null,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            MySqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<MySqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                new CommandDefinition(
                    storeProcedureName,
                    parameters,
                    transaction,
                    null,
                    CommandType.StoredProcedure,
                    cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            MySqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            MySqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<MySqlConnection> mappingConfig,
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
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            string connectionString,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        null,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<MySqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task CallStoredProcedureAsync(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<MySqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            var parameters = MapToDynamicParameters(parametersMap);
            await connection.ExecuteAsync(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            return connection.QueryAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return connection.QueryAsync<T>(
                new CommandDefinition(
                    storeProcedureName,
                    parameters,
                    transaction,
                    null,
                    CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            MySqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return connection.QueryAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            MySqlConnection connection,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            return connection.QueryAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        null,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            MySqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<MySqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryAsync<T>(
                new CommandDefinition(
                    storeProcedureName,
                    parameters,
                    transaction,
                    null,
                    CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            MySqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryAsync<T>(
                new CommandDefinition(
                    storeProcedureName,
                    parameters,
                    null,
                    null,
                    CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            MySqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<MySqlConnection> mappingConfig,
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
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        null,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<MySqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> CallStoredProcedureAsync<T>(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<MySqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return connection.QueryFirstOrDefaultAsync<T>(
                new CommandDefinition(
                    storeProcedureName,
                    parameters,
                    transaction,
                    null,
                    CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            MySqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            MySqlConnection connection,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        null,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            MySqlConnection connection,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<MySqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        transaction,
                        commandTimeout,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryFirstOrDefaultAsync<T>(
                new CommandDefinition(
                    storeProcedureName,
                    parameters,
                    transaction,
                    null,
                    CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            MySqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            var parameters = MapToDynamicParameters(parametersMap);
            return connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            MySqlConnection connection,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<MySqlConnection> mappingConfig,
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
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string connectionString,
            string storeProcedureName,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        null,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string connectionString,
            string storeProcedureName,
            DynamicParameters parameters,
            Action<MySqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(
            string connectionString,
            string storeProcedureName,
            Dictionary<string, object> parametersMap,
            Action<MySqlConnection> mappingConfig,
            CancellationToken cancellationToken)
        {
            await using var connection = new MySqlConnection(connectionString);
            if (mappingConfig != null)
            {
                await connection.OpenAsync(cancellationToken);
                mappingConfig.Invoke(connection);
            }

            var parameters = MapToDynamicParameters(parametersMap);
            return await connection.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        storeProcedureName,
                        parameters,
                        null,
                        null,
                        CommandType.StoredProcedure,
                        cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        public static async Task<List<T>> QueryAsync<T>(
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
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
            MySqlConnection connection,
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
            MySqlTransaction transaction,
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
            MySqlConnection connection,
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
            MySqlConnection connection,
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
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
            await using var connection = new MySqlConnection(connectionString);
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
            throw new NotSupportedException($"Не удалось создать функцию {functionName}. Табличные фунции не поддерживаются в mysql. Используйте хранимые процедуры.");
        }

        private static string BuildTableFunctionSql(string functionName, DynamicParameters parameters)
        {
            throw new NotSupportedException($"Не удалось создать функцию {functionName}({string.Join(",", parameters.ParameterNames.Select(p => $"@{p}"))}). Табличные фунции не поддерживаются в mysql. Используйте хранимые процедуры.");
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
