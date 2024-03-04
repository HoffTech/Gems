using Gems.TestInfrastructure.Environment;

using Testcontainers.PostgreSql;

namespace Gems.TestInfrastructure.Postgres.Environment
{
    internal class PostgresDatabaseContainer : IDatabaseContainer
    {
        private readonly PostgreSqlContainer container;

        public PostgresDatabaseContainer(PostgreSqlContainer container)
        {
            this.container = container;
        }

        public string ConnectionString => this.container.GetConnectionString();
    }
}
