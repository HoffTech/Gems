// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;

namespace Gems.Metrics.Contracts
{
    public struct MetricInfo : IEquatable<MetricInfo>
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

        public bool Equals(MetricInfo other)
        {
            return this.Name.Equals(other.Name, StringComparison.Ordinal) &&
                   this.LabelValues.SequenceEqual(other.LabelValues);
        }

        public override bool Equals(object obj)
        {
            return obj is MetricInfo other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Name, string.Join(',', this.LabelValues));
        }

        public override string ToString()
        {
            return $"{this.Name}; {string.Join(',', this.LabelNames)}; {string.Join(',', this.LabelValues)}";
        }
    }
}
