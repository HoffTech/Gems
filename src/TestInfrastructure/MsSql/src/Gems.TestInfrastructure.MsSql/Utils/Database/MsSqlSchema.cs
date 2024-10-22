// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Utils.Database;

namespace Gems.TestInfrastructure.MsSql.Utils.Database
{
    public class MsSqlSchema : Schema
    {
        private readonly string currentDatabase;

        internal MsSqlSchema(
            List<DatabaseMetadata> databases,
            List<TableMetadata> tables,
            List<ColumnMetadata> columns,
            List<UserMetadata> users,
            List<IndexMetadata> indexes,
            List<IndexColumnMetadata> indexColumns,
            string currentDatabase) : base(databases, tables, columns, users, indexes, indexColumns)
        {
            this.currentDatabase = currentDatabase;
        }

        public DatabaseMetadata CurrentDatabase => this.Databases.FirstOrDefault(x => x.DatabaseName == this.currentDatabase);
    }
}
