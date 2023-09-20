// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Metrics.Contracts
{
    internal class MetricLifeInfo
    {
        public MetricLifeInfo(MetricInfo metricInfo, DateTime createdDate)
        {
            this.Name = metricInfo.Name;
            this.Description = metricInfo.Description;
            this.LabelValues = metricInfo.LabelValues;
            this.LabelNames = metricInfo.LabelNames;
            this.CreatedDate = createdDate;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string[] LabelValues { get; set; }

        public string[] LabelNames { get; set; }

        public DateTime CreatedDate { get; set; }

        public MetricInfo MapToMetricInfo()
        {
            return new MetricInfo
            {
                Name = this.Name,
                Description = this.Description,
                LabelNames = this.LabelNames,
                LabelValues = this.LabelValues
            };
        }
    }
}
