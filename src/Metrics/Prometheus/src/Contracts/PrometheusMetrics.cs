// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Metrics.Prometheus.Contracts
{
    /// <summary>
    /// Содержит конфигурацию для метрик Prometheus.
    /// </summary>
    public class PrometheusMetrics
    {
        /// <summary>
        /// Конфигурация метрик.
        /// </summary>
        public Configuration Configuration { get; set; }
    }
}
