// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Logging.Mvc.LogsCollector;
using Gems.Logging.Security;

using Microsoft.Extensions.DependencyInjection;

namespace Gems.Logging.Mvc
{
    public static class ServiceCollectionExtensions
    {
        private const string SecureKeyJsonHttpSourceUrl = "http://api-dev.kifr-ru.local/securitykeys/static/log-filter.json";

        public static void AddSecureLogging(this IServiceCollection services)
        {
            services.AddSingleton<IRequestLogsCollectorFactory, SecureRequestLogsCollectorFactory>();
            services.AddLoggingFilter(builder => builder.Register(new SecureKeyJsonHttpSource(new Uri(SecureKeyJsonHttpSourceUrl))));
        }
    }
}
