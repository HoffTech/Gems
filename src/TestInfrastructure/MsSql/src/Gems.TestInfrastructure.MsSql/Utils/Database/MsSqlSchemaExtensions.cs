using System.Data.SqlClient;

using Gems.TestInfrastructure.Utils.Database;

namespace Gems.TestInfrastructure.MsSql.Utils.Database
{
    public static class MsSqlSchemaExtensions
    {
        public static async Task<MsSqlSchema> SchemaAsync(this SqlConnection connection, CancellationToken cancellationToken = default)
        {
            var databases = await connection.GetDatabasesAsync(cancellationToken);
            var tables = await connection.GetTablesAsync(cancellationToken);
            var columns = await connection.GetColumnsAsync(cancellationToken);
            var users = await connection.GetUsersAsync(cancellationToken);
            var currentDatabase = await connection.GetCurrentDatabaseAsync(cancellationToken);
            var indexes = await connection.GetIndexesAsync(cancellationToken);
            var indexColumns = await connection.GetIndexColumnsAsync(cancellationToken);
            var schema = new MsSqlSchema(databases, tables, columns, users, indexes, indexColumns, currentDatabase);
            return schema;
        }

        public static MsSqlSchema Schema(this SqlConnection connection)
        {
            return connection.SchemaAsync(default)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
    }
}
