// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Logging.Mvc.LogsCollector;
using Gems.Logging.Security;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Logging.Mvc
{
    public static class ServiceCollectionExtensions
    {
        private const string SecureKeyJsonHttpSourceUrl = "http://api-dev.kifr-ru.local/securitykeys/static/log-filter.json";

        public static void AddSecureLogging(this IServiceCollection services)
        {
            AddSecureLogging(services, null);
        }

        public static void AddSecureLogging(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration != null)
            {
                services.Configure<RequestLogsCollectorOptions>(configuration.GetSection(RequestLogsCollectorOptions.Name));
            }

            services.AddSingleton<IRequestLogsCollectorFactory, SecureRequestLogsCollectorFactory>();
            services.AddLoggingFilter(builder => builder.Register(new SecureKeyJsonHttpSource(new Uri(SecureKeyJsonHttpSourceUrl))));
        }
    }
}
