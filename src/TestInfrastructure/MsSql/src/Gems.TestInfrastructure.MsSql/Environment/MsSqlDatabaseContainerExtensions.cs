using System.Data.SqlClient;

using Gems.TestInfrastructure.Environment;

namespace Gems.TestInfrastructure.MsSql.Environment
{
    public static class MsSqlDatabaseContainerExtensions
    {
        public static async Task<SqlConnection> ConnectMsSqlAsync(
            this ITestEnvironment env,
            string name,
            CancellationToken cancellationToken = default)
        {
            var connection = new SqlConnection(env.DatabaseConnectionString(name));
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

        public static async Task<SqlConnection> ConnectMsSqlAsync(
            this IDatabaseContainer container,
            CancellationToken cancellationToken = default)
        {
            var connection = new SqlConnection(container.ConnectionString);
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
