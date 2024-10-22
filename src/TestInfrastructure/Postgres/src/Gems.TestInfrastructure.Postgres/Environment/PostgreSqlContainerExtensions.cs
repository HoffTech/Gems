// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Postgres.Utils.Database;

using Npgsql;

using Testcontainers.PostgreSql;

namespace Gems.TestInfrastructure.Postgres.Environment
{
    public static class PostgreSqlContainerExtensions
    {
        public static async Task ExecScriptAsync(this PostgreSqlContainer c, FileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            using var stream = fileInfo.OpenText();
            await c.ExecScriptAsync(await stream.ReadToEndAsync(), cancellationToken);
        }

        public static async Task SetupAsync(
            this PostgreSqlContainer c,
            Func<NpgsqlConnection, PostgresSchema, Task> setup,
            CancellationToken cancellationToken = default)
        {
            await c.DoAsync(async connection => await setup(connection, await connection.SchemaAsync(cancellationToken)), cancellationToken);
        }

        public static async Task DoAsync(
            this PostgreSqlContainer c,
            Func<NpgsqlConnection, Task> action,
            CancellationToken cancellationToken = default)
        {
            await using var connection = new NpgsqlConnection(c.GetConnectionString());
            await connection.OpenAsync(cancellationToken);
            await action(connection);
        }
    }
}
