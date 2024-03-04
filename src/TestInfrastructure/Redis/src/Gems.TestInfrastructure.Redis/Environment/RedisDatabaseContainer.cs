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
