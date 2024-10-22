// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Environment;

using Npgsql;

namespace Gems.TestInfrastructure.Postgres.Environment
{
    public static class PostgresDatabaseContainerExtensions
    {
        public static async Task<NpgsqlConnection> ConnectPostgresAsync(
            this ITestEnvironment env,
            string name,
            CancellationToken cancellationToken = default)
        {
            var connection = new NpgsqlConnection(env.DatabaseConnectionString(name));
            try
            {
                await connection.OpenAsync(cancellationToken);
                return connection;
            }
            catch
            {
                await connection.DisposeAsync();
                throw;
            }
        }

        public static async Task<NpgsqlConnection> ConnectPostgresAsync(
            this IDatabaseContainer container,
            CancellationToken cancellationToken = default)
        {
            var connection = new NpgsqlConnection(container.ConnectionString);
            try
            {
                await connection.OpenAsync(cancellationToken);
                return connection;
            }
            catch
            {
                await connection.DisposeAsync();
                throw;
            }
        }
    }
}
