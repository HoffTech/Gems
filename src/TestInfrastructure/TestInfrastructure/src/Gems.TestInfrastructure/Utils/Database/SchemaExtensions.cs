// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Data.Common;

namespace Gems.TestInfrastructure.Utils.Database
{
    public static class SchemaExtensions
    {
        public static async Task<Schema> SchemaAsync(this DbConnection connection, CancellationToken cancellationToken = default)
        {
            var databases = await connection.GetDatabasesAsync(cancellationToken);
            var tables = await connection.GetTablesAsync(cancellationToken);
            var columns = await connection.GetColumnsAsync(cancellationToken);
            var users = await connection.GetUsersAsync(cancellationToken);
            var indexes = await connection.GetIndexesAsync(cancellationToken);
            var indexColumns = await connection.GetIndexColumnsAsync(cancellationToken);
            return new Schema(databases, tables, columns, users, indexes, indexColumns);
        }

        public static Schema Schema(this DbConnection connection)
        {
            return connection.SchemaAsync(default)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public static UserMetadata User(this Schema schema, string name) =>
            schema.Users.First(x => x.Equals(name));

        public static UserMetadata TryGetUser(this Schema schema, string name) =>
            schema.Users.FirstOrDefault(x => x.Equals(name));

        public static DatabaseMetadata Database(this Schema schema, string name) =>
            schema.Databases.First(x => x.Equals(name));

        public static DatabaseMetadata TryGetDatabase(this Schema schema, string name) =>
            schema.Databases.FirstOrDefault(x => x.Equals(name));

        public static TableMetadata Table(this DatabaseMetadata database, string name) =>
            database.Tables.First(x => x.Equals(name));

        public static TableMetadata TryGetTable(this DatabaseMetadata database, string name) =>
            database.Tables.FirstOrDefault(x => x.Equals(name));

        public static IndexMetadata Index(this TableMetadata table, string name) =>
            table.Indexes.First(x => x.Equals(name));

        public static IndexMetadata TryGetIndex(this TableMetadata table, string name) =>
            table.Indexes.FirstOrDefault(x => x.Equals(name));

        public static ColumnMetadata Column(this TableMetadata table, string name) =>
            table.Columns.First(x => x.Equals(name));

        public static ColumnMetadata TryGetColumn(this TableMetadata table, string name) =>
            table.Columns.FirstOrDefault(x => x.Equals(name));

        public static IndexColumnMetadata Column(this IndexMetadata table, string name) =>
            table.Columns.First(x => x.Equals(name));

        public static IndexColumnMetadata TryGetColumn(this IndexMetadata table, string name) =>
            table.Columns.FirstOrDefault(x => x.Equals(name));
    }
}
