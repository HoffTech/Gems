// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Utils.Database
{
    public class Schema
    {
        private readonly List<UserMetadata> users;
        private readonly List<DatabaseMetadata> databases;

        public Schema(
            List<DatabaseMetadata> databases,
            List<TableMetadata> tables,
            List<ColumnMetadata> columns,
            List<UserMetadata> users,
            List<IndexMetadata> indexes,
            List<IndexColumnMetadata> indexColumns)
        {
            this.databases = databases;
            this.users = users;
            indexes.ForEach(i => i.Columns = indexColumns
                .Where(x =>
                    x.TableCatalog == i.TableCatalog &&
                    x.TableSchema == i.TableSchema &&
                    x.TableName == i.TableName &&
                    x.IndexName == i.IndexName)
                .ToList());
            foreach (var database in this.databases)
            {
                database.Tables = tables
                    .Where(t => t.TableCatalog == database.DatabaseName)
                    .ToList();
                database.Tables.ForEach(t =>
                {
                    t.Columns = columns
                        .Where(c =>
                            c.TableCatalog == t.TableCatalog &&
                            c.TableSchema == t.TableSchema &&
                            c.TableName == t.TableName)
                        .ToList();
                    t.Indexes = indexes
                        .Where(c =>
                            c.TableCatalog == t.TableCatalog &&
                            c.TableSchema == t.TableSchema &&
                            c.TableName == t.TableName)
                        .ToList();
                });
            }
        }

        public List<DatabaseMetadata> Databases => this.databases;

        public List<UserMetadata> Users => this.users;
    }
}
