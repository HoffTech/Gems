using Gems.TestInfrastructure.Utils.Database;

namespace Gems.TestInfrastructure.Postgres.Utils.Database
{
    public class PostgresSchema : Schema
    {
        private readonly string currentDatabase;

        internal PostgresSchema(
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
