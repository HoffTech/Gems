// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Environment;

using Testcontainers.Redis;

namespace Gems.TestInfrastructure.Redis.Environment
{
    internal class RedisDatabaseContainer : IDatabaseContainer
    {
        private readonly RedisContainer container;

        public RedisDatabaseContainer(RedisContainer container)
        {
            this.container = container;
        }

        public string ConnectionString => this.container.GetConnectionString();
    }
}
