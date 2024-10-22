// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Data.SqlClient;

using Gems.TestInfrastructure.MsSql.Utils.Database;

using Testcontainers.MsSql;

namespace Gems.TestInfrastructure.MsSql.Environment
{
    public static class MsSqlContainerExtensions
    {
        public static async Task ExecScriptAsync(this MsSqlContainer c, FileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            using var stream = fileInfo.OpenText();
            await c.ExecScriptAsync(await stream.ReadToEndAsync(), cancellationToken);
        }

        public static async Task SetupAsync(
            this MsSqlContainer c,
            Func<SqlConnection, MsSqlSchema, Task> setup,
            CancellationToken cancellationToken = default)
        {
            await c.DoAsync(async connection => await setup(connection, await connection.SchemaAsync(cancellationToken)), cancellationToken);
        }

        public static async Task DoAsync(
            this MsSqlContainer c,
            Func<SqlConnection, Task> action,
            CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(c.GetConnectionString());
            await connection.OpenAsync(cancellationToken);
            await action(connection);
        }
    }
}
