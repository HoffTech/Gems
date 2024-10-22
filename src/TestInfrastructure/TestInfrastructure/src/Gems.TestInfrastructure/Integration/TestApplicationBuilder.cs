// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gems.TestInfrastructure.Integration
{
    public class TestApplicationBuilder<TEntryPoint> : ITestApplicationBuilder
        where TEntryPoint : class
    {
        private readonly TestApplicationConfiguration configuration;

        public TestApplicationBuilder()
        {
            this.configuration = new TestApplicationConfiguration();
        }

        public ITestApplication Build()
        {
            return new TestApplication<TEntryPoint>(this.configuration);
        }

        public ITestApplicationBuilder ConfigureWebHost(Action<IWebHostBuilder> setup)
        {
            this.configuration.WebHostBuilders.Add(setup);
            return this;
        }

        public ITestApplicationBuilder UseEnvironment(string environment)
        {
            this.configuration.WebHostBuilders.Add(builder => builder.UseEnvironment(environment));
            return this;
        }

        public ITestApplicationBuilder UseSetting(string name, string value)
        {
            this.configuration.WebHostBuilders.Add(builder => builder.UseSetting(name, value));
            return this;
        }

        public ITestApplicationBuilder UseConfiguration(Func<IConfiguration> configurationFactory)
        {
            this.configuration.WebHostBuilders.Add(builder => builder.UseConfiguration(configurationFactory()));
            return this;
        }

        public ITestApplicationBuilder ConfigureLogging(Action<ILoggingBuilder> build)
        {
            this.configuration.WebHostBuilders.Add(b => b.ConfigureLogging(build));
            return this;
        }

        public ITestApplicationBuilder ConfigureServices(Action<IServiceCollection> setup)
        {
            this.configuration.WebHostBuilders.Add(b => b.ConfigureServices(setup));
            return this;
        }

        public ITestApplicationBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> setup)
        {
            this.configuration.WebHostBuilders.Add(b => b.ConfigureAppConfiguration(setup));
            return this;
        }
    }
}
