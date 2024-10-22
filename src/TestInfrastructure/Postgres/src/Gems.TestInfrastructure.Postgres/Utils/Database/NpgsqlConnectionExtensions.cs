// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Dapper;

using Npgsql;

namespace Gems.TestInfrastructure.Postgres.Utils.Database
{
    public static class NpgsqlConnectionExtensions
    {
        public static Task<string> GetCurrentDatabaseAsync(
            this NpgsqlConnection connection,
            CancellationToken cancellationToken = default) =>
                connection.QuerySingleAsync<string>(new CommandDefinition(
                    "SELECT current_database();",
                    cancellationToken: cancellationToken));
    }
}
