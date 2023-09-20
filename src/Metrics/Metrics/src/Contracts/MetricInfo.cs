// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Metrics.Contracts
{
    public struct MetricInfo
    {
        /// <summary>
        /// Описание метрики.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Название метрики.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Название меток.
        /// </summary>
        public string[] LabelNames { get; set; }

        /// <summary>
        /// Значения для установки меток.
        /// </summary>
        public string[] LabelValues { get; set; }
    }
}
