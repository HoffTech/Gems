using Microsoft.AspNetCore.Hosting;

namespace Gems.TestInfrastructure.Integration
{
    internal class TestApplicationConfiguration
    {
        public List<Action<IWebHostBuilder>> WebHostBuilders { get; } = new List<Action<IWebHostBuilder>>();
    }
}
