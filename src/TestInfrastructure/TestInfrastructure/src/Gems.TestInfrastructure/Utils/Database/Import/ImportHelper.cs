// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Data.Common;
using System.Text;

namespace Gems.TestInfrastructure.Utils.Database.Import
{
    public static class ImportHelper
    {
        public static async Task ImportAsync(
            DbConnection connection,
            TableMetadata table,
            IAsyncEnumerable<IImportRow> rows,
            string parameterPrefix,
            Func<DbCommand> commandFactory,
            Func<string, object, DbParameter> parameterFactory,
            CancellationToken cancellationToken = default)
        {
            var columnMap = new List<Tuple<int, string>>();
            DbCommand command = null;
            try
            {
                await foreach (var row in rows)
                {
                    if (command == null)
                    {
                        var sourceColumns = row.GetColumns();
                        for (var i = 0; i < sourceColumns.Length; i++)
                        {
                            var targetColumn = table.Columns.FirstOrDefault(x => x.ColumnName.Equals(sourceColumns[i]));
                            if (targetColumn != null)
                            {
                                columnMap.Add(new Tuple<int, string>(i, targetColumn.ColumnName));
                            }
                        }

                        if (columnMap.Count == 0)
                        {
                            break;
                        }

                        var sqlBuilder = new StringBuilder();
                        sqlBuilder
                            .Append("INSERT INTO ")
                            .Append(table.TableSchema)
                            .Append('.')
                            .Append(table.TableName)
                            .Append(" (")
                            .Append(string.Join(", ", columnMap.Select(x => x.Item2)))
                            .Append(") VALUES (")
                            .Append(string.Join(", ", columnMap.Select(x => $"{parameterPrefix}{x.Item2}")))
                            .Append(");");
                        command = commandFactory();
                        command.CommandText = sqlBuilder.ToString();
                        command.Connection = connection;
                    }

                    command.Parameters.Clear();
                    columnMap.ForEach(x => command.Parameters.Add(parameterFactory(
                        $"{parameterPrefix}{x.Item2}",
                        row.GetValue(x.Item1) ?? DBNull.Value)));
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }
        }
    }
}
