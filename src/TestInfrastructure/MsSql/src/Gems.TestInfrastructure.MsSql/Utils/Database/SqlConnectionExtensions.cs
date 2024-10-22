// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Data.SqlClient;

using Dapper;

namespace Gems.TestInfrastructure.MsSql.Utils.Database
{
    public static class SqlConnectionExtensions
    {
        public static Task<string> GetCurrentDatabaseAsync(
            this SqlConnection connection,
            CancellationToken cancellationToken = default) =>
                connection.QuerySingleAsync<string>(new CommandDefinition(
                    "SELECT DB_NAME();",
                    cancellationToken: cancellationToken));
    }
}
