// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Metrics.Contracts
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MetricAttribute : Attribute
    {
        public string Description { get; set; }

        public string Name { get; set; }

        public string[] LabelValues { get; set; }

        public string[] LabelNames { get; set; }
    }
}
