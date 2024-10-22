// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Data;
using System.Data.Common;

namespace Gems.TestInfrastructure.Utils.Database
{
    public static class DbConnectionExtensions
    {
        public static async Task<List<TableMetadata>> GetTablesAsync(this DbConnection connection, CancellationToken cancellationToken)
        {
            using var collection = await connection.GetSchemaAsync("Tables", cancellationToken);
            var result = new List<TableMetadata>();
            foreach (DataRow row in collection.Rows)
            {
                result.Add(new TableMetadata
                {
                    TableCatalog = row.GetValue<string>("table_catalog"),
                    TableName = row.GetValue<string>("table_name"),
                    TableSchema = row.GetValue<string>("table_schema"),
                    TableType = row.GetValue<string>("table_type"),
                });
            }

            return result;
        }

        public static async Task<List<IndexMetadata>> GetIndexesAsync(this DbConnection connection, CancellationToken cancellationToken)
        {
            using var collection = await connection.GetSchemaAsync("Indexes", cancellationToken);
            var result = new List<IndexMetadata>();
            foreach (DataRow row in collection.Rows)
            {
                result.Add(new IndexMetadata
                {
                    TableCatalog = row.GetValue<string>("table_catalog"),
                    TableSchema = row.GetValue<string>("table_schema"),
                    TableName = row.GetValue<string>("table_name"),
                    IndexName = row.GetValue<string>("index_name"),
                    TypeDesc = row.GetValue<string>("type_desc"),
                });
            }

            return result;
        }

        public static async Task<List<IndexColumnMetadata>> GetIndexColumnsAsync(this DbConnection connection, CancellationToken cancellationToken)
        {
            using var collection = await connection.GetSchemaAsync("IndexColumns", cancellationToken);
            var result = new List<IndexColumnMetadata>();
            foreach (DataRow row in collection.Rows)
            {
                result.Add(new IndexColumnMetadata
                {
                    TableCatalog = row.GetValue<string>("table_catalog"),
                    TableSchema = row.GetValue<string>("table_schema"),
                    TableName = row.GetValue<string>("table_name"),
                    ColumnName = row.GetValue<string>("column_name"),
                    IndexName = row.GetValue<string>("index_name"),
                });
            }

            return result;
        }

        public static async Task<List<DatabaseMetadata>> GetDatabasesAsync(this DbConnection connection, CancellationToken cancellationToken)
        {
            using var collection = await connection.GetSchemaAsync("Databases", cancellationToken);
            var result = new List<DatabaseMetadata>();
            foreach (DataRow row in collection.Rows)
            {
                result.Add(new DatabaseMetadata
                {
                    DatabaseName = row.GetValue<string>("database_name"),
                    Owner = row.GetValue<string>("owner"),
                    Encoding = row.GetValue<string>("encoding"),
                });
            }

            return result;
        }

        public static async Task<List<UserMetadata>> GetUsersAsync(this DbConnection connection, CancellationToken cancellationToken)
        {
            using var collection = await connection.GetSchemaAsync("Users", cancellationToken);
            var result = new List<UserMetadata>();
            foreach (DataRow row in collection.Rows)
            {
                result.Add(new UserMetadata
                {
                    UserName = row.GetValue<string>("user_name"),
                    UserSysid = row.GetValue<uint>("user_sysid"),
                });
            }

            return result;
        }

        public static async Task<List<ColumnMetadata>> GetColumnsAsync(this DbConnection connection, CancellationToken cancellationToken)
        {
            using var collection = await connection.GetSchemaAsync("Columns", cancellationToken);
            var result = new List<ColumnMetadata>();
            foreach (DataRow row in collection.Rows)
            {
                result.Add(new ColumnMetadata
                {
                    TableCatalog = row.GetValue<string>("table_catalog"),
                    TableSchema = row.GetValue<string>("table_schema"),
                    TableName = row.GetValue<string>("table_name"),
                    ColumnName = row.GetValue<string>("column_name"),
                    OrdinalPosition = row.GetValue<int>("ordinal_position"),
                    ColumnDefault = row.GetValue<string>("column_default"),
                    IsNullable = row.GetValue<string>("is_nullable"),
                    DataType = row.GetValue<string>("data_type"),
                    CharacterMaximumLength = row.GetValue<int>("character_maximum_length"),
                    CharacterOctetLength = row.GetValue<int>("character_octet_length"),
                    NumericPrecision = row.GetValue<int>("numeric_precision"),
                    NumericPrecisionRadix = row.GetValue<int>("numeric_precision_radix"),
                    NumericScale = row.GetValue<int>("numeric_scale"),
                    DatetimePrecision = row.GetValue<int>("datetime_precision"),
                    CharacterSetCatalog = row.GetValue<string>("character_set_catalog"),
                    CharacterSetSchema = row.GetValue<string>("character_set_schema"),
                    CharacterSetName = row.GetValue<string>("character_set_name"),
                    CollationCatalog = row.GetValue<string>("collation_catalog"),
                });
            }

            return result;
        }
    }
}
