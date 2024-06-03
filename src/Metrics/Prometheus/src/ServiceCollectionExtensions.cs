// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Metrics.LabelsProvider;
using Gems.Metrics.MetricDecorators;
using Gems.Metrics.Prometheus.Contracts;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Metrics.Prometheus
{
    /// <summary>
    /// Class with middleware extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add prometheus.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        /// <param name="configuration">IConfiguration.</param>
        public static void AddPrometheus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PrometheusMetrics>(configuration.GetSection("PrometheusMetrics"));

            services.AddSingleton<MetricsService>();
            services.AddSingleton<LifeMetricsServiceDecorator>();
            services.AddSingleton<LabelsProviderSelector>();
            services.AddSingleton<IMetricsService>(s =>
            {
                var lifeMetricsServiceDecorator = s.GetService<LifeMetricsServiceDecorator>();
                lifeMetricsServiceDecorator?.SetComponent(s.GetService<MetricsService>());
                return lifeMetricsServiceDecorator;
            });

            services.AddHostedService<ResetMetricsHostedService>();
        }
    }
}
