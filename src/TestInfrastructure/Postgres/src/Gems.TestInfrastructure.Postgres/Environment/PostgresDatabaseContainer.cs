// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

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
