// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gems.TestInfrastructure.Integration
{
    public interface ITestApplicationBuilder
    {
        ITestApplication Build();

        ITestApplicationBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> setup);

        ITestApplicationBuilder ConfigureLogging(Action<ILoggingBuilder> build);

        ITestApplicationBuilder ConfigureServices(Action<IServiceCollection> setup);

        ITestApplicationBuilder ConfigureWebHost(Action<IWebHostBuilder> setup);

        ITestApplicationBuilder UseConfiguration(Func<IConfiguration> configurationFactory);

        ITestApplicationBuilder UseEnvironment(string environment);

        ITestApplicationBuilder UseSetting(string name, string value);
    }
}
