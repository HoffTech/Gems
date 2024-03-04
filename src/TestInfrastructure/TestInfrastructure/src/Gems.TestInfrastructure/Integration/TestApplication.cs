using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gems.TestInfrastructure.Integration
{
    internal class TestApplication<TEntryPoint> : WebApplicationFactory<TEntryPoint>, ITestApplication
        where TEntryPoint : class
    {
        private readonly Lazy<HttpClient> httpClient;
        private readonly TestApplicationConfiguration configuration;

        public TestApplication(TestApplicationConfiguration configuration)
        {
            this.httpClient = new Lazy<HttpClient>(this.CreateClient);
            this.configuration = configuration;
        }

        public HttpClient HttpClient => this.httpClient.Value;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            this.configuration.WebHostBuilders.ForEach(webHostBuilder => webHostBuilder(builder));
            base.ConfigureWebHost(builder);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.httpClient.IsValueCreated)
                {
                    this.httpClient.Value.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
