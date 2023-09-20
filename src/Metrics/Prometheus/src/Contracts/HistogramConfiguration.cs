// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Metrics.Prometheus.Contracts
{
    /// <summary>
    /// Конфигурация для Гистограммы.
    /// </summary>
    public class HistogramConfiguration : MetricConfiguration
    {
        /// <summary>
        /// Шкала гистограммы.
        /// </summary>
        public double[] Buckets { get; set; }
    }
}
