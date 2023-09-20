// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Metrics.Prometheus.Contracts
{
    /// <summary>
    /// Конфигурация метрик.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets or sets конфигурацию для Гистограммы.
        /// </summary>
        public Dictionary<string, MetricConfiguration> HistogramConfiguration { get; set; }

        /// <summary>
        /// Gets or sets конфигурации для Измерений.
        /// </summary>
        public Dictionary<string, MetricConfiguration> GaugeConfiguration { get; set; }

        /// <summary>
        /// Gets or sets конфигурации для Счетчиков.
        /// </summary>
        public Dictionary<string, MetricConfiguration> CounterConfiguration { get; set; }
    }
}
