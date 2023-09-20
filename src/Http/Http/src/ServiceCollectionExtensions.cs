// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Http
{
    /// <summary>
    ///     Class with middleware extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Add prometheus.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="configureOptions">configureOptions.</param>
        public static void AddHttpServices(this IServiceCollection services, IConfiguration configuration, Action<HttpClientServiceOptions> configureOptions = null)
        {
            services.Configure<HttpClientServiceOptions>(options =>
            {
                configuration.GetSection(nameof(HttpClientServiceOptions)).Bind(options);
                configureOptions?.Invoke(options);
            });
            services.AddSingleton<IHttpClientFactory, DefaultHttpClientFactory>();
            services.AddSingleton<DefaultClientService>();
            services.AddSingleton<BaseClientServiceHelper>();
        }
    }
}
