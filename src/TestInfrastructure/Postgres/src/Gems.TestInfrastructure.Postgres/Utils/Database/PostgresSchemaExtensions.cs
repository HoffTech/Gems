// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Utils.Database;

using Npgsql;

namespace Gems.TestInfrastructure.Postgres.Utils.Database
{
    public static class PostgresSchemaExtensions
    {
        public static async Task<PostgresSchema> SchemaAsync(this NpgsqlConnection connection, CancellationToken cancellationToken = default)
        {
            var databases = await connection.GetDatabasesAsync(cancellationToken);
            var tables = await connection.GetTablesAsync(cancellationToken);
            var columns = await connection.GetColumnsAsync(cancellationToken);
            var users = await connection.GetUsersAsync(cancellationToken);
            var currentDatabase = await connection.GetCurrentDatabaseAsync(cancellationToken);
            var indexes = await connection.GetIndexesAsync(cancellationToken);
            var indexColumns = await connection.GetIndexColumnsAsync(cancellationToken);
            var schema = new PostgresSchema(databases, tables, columns, users, indexes, indexColumns, currentDatabase);
            return schema;
        }

        public static PostgresSchema Schema(this NpgsqlConnection connection)
        {
            return connection.SchemaAsync(default)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}
