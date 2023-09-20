// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Metrics.Prometheus.Contracts
{
    /// <summary>
    /// Конфигурация метрик.
    /// </summary>
    public class MetricConfiguration
    {
        /// <summary>
        /// Gets or sets статические метки, применяемые ко всем экземплярам этой метрики. Эти метки не могут быть впоследствии перезаписаны.
        /// </summary>
        public Dictionary<string, string> StaticLabels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether сжатие начального значения.
        /// Если значение true, то метрика не будет опубликована до тех пор, пока ее значение не будет изменено.
        /// </summary>
        public bool SuppressInitialValue { get; set; }

        /// <summary>
        /// Gets or sets имена всех полей меток, определенных для каждого экземпляра метрики.
        /// Если значение равно null, метрика будет создана без каких-либо меток для конкретного экземпляра.
        /// </summary>
        public string[] LabelNames { get; set; }
    }
}
