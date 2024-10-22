// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Environment;

using Testcontainers.MsSql;

namespace Gems.TestInfrastructure.MsSql.Environment
{
    internal class MsSqlDatabaseContainer : IDatabaseContainer
    {
        private readonly MsSqlContainer container;

        public MsSqlDatabaseContainer(MsSqlContainer container)
        {
            this.container = container;
        }

        public string ConnectionString => this.container.GetConnectionString();
    }
}
