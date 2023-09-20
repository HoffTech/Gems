// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

using Gems.Metrics.Contracts;

namespace Gems.Metrics.MetricDecorators
{
    public class AMetricsServiceDecorator : IMetricsService
    {
#pragma warning disable SA1401 // Fields should be private
        protected IMetricsService component;
#pragma warning restore SA1401 // Fields should be private

        public void SetComponent(IMetricsService component)
        {
            this.component = component;
        }

        /// <inheritdoc />
        public virtual Task Counter(string name, string description, double increment = 1, params string[] labelValues)
        {
            return this.component.Counter(name, description, increment, labelValues);
        }

        /// <inheritdoc />
        public virtual Task Counter(string name, double increment = 1, params string[] labelValues)
        {
            return this.component.Counter(name, increment, labelValues);
        }

        /// <inheritdoc />
        public virtual Task Counter(Enum enumValue, double increment = 1, params string[] labelValues)
        {
            return this.component.Counter(enumValue, increment, labelValues);
        }

        /// <inheritdoc />
        public virtual Task Counter(MetricInfo metricInfo, double increment = 1)
        {
            return this.component.Counter(metricInfo, increment);
        }

        /// <inheritdoc />
        public virtual Task CounterTo(string name, string description, double targetValue, params string[] labelValues)
        {
            return this.component.CounterTo(name, description, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task CounterTo(string name, double targetValue, params string[] labelValues)
        {
            return this.component.CounterTo(name, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task CounterTo(Enum enumValue, double targetValue, params string[] labelValues)
        {
            return this.component.CounterTo(enumValue, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task CounterTo(MetricInfo metricInfo, double targetValue)
        {
            return this.component.CounterTo(metricInfo, targetValue);
        }

        /// <inheritdoc />
        public virtual Task Gauge(string name, string description, double increment = 1, params string[] labelValues)
        {
            return this.component.Gauge(name, description, increment, labelValues);
        }

        /// <inheritdoc />
        public virtual Task Gauge(string name, double increment = 1, params string[] labelValues)
        {
            return this.component.Gauge(name, increment, labelValues);
        }

        /// <inheritdoc />
        public virtual Task Gauge(Enum enumValue, double increment = 1, params string[] labelValues)
        {
            return this.component.Gauge(enumValue, increment, labelValues);
        }

        /// <inheritdoc />
        public virtual Task Gauge(MetricInfo metricInfo, double increment = 1)
        {
            return this.component.Gauge(metricInfo, increment);
        }

        /// <inheritdoc />
        public virtual Task GaugeDec(string name, string description, double decrement = 1, params string[] labelValues)
        {
            return this.component.GaugeDec(name, description, decrement, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeDec(string name, double decrement = 1, params string[] labelValues)
        {
            return this.component.GaugeDec(name, decrement, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeDec(Enum enumValue, double decrement = 1, params string[] labelValues)
        {
            return this.component.GaugeDec(enumValue, decrement, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeDec(MetricInfo metricInfo, double decrement = 1)
        {
            return this.component.GaugeDec(metricInfo, decrement);
        }

        /// <inheritdoc />
        public virtual Task GaugeDecTo(string name, string description, double targetValue, params string[] labelValues)
        {
            return this.component.GaugeDecTo(name, description, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeDecTo(string name, double targetValue, params string[] labelValues)
        {
            return this.component.GaugeDecTo(name, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeDecTo(Enum enumValue, double targetValue, params string[] labelValues)
        {
            return this.component.GaugeDecTo(enumValue, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeDecTo(MetricInfo metricInfo, double targetValue)
        {
            return this.component.GaugeDecTo(metricInfo, targetValue);
        }

        /// <inheritdoc />
        public virtual Task GaugeIncTo(string name, string description, double targetValue, params string[] labelValues)
        {
            return this.component.GaugeIncTo(name, description, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeIncTo(string name, double targetValue, params string[] labelValues)
        {
            return this.component.GaugeIncTo(name, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeIncTo(Enum enumValue, double targetValue, params string[] labelValues)
        {
            return this.component.GaugeIncTo(enumValue, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeIncTo(MetricInfo metricInfo, double targetValue)
        {
            return this.component.GaugeIncTo(metricInfo, targetValue);
        }

        /// <inheritdoc />
        public virtual Task GaugeSet(string name, string description, double targetValue, params string[] labelValues)
        {
            return this.component.GaugeSet(name, description, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeSet(string name, double targetValue, params string[] labelValues)
        {
            return this.component.GaugeSet(name, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeSet(Enum enumValue, double targetValue, params string[] labelValues)
        {
            return this.component.GaugeSet(enumValue, targetValue, labelValues);
        }

        /// <inheritdoc />
        public virtual Task GaugeSet(MetricInfo metricInfo, double targetValue)
        {
            return this.component.GaugeSet(metricInfo, targetValue);
        }

        /// <inheritdoc />
        public virtual Task Histogram(string name, string description, double value, params string[] labelValues)
        {
            return this.component.Histogram(name, description, value, labelValues);
        }

        /// <inheritdoc />
        public virtual Task Histogram(string name, double value, params string[] labelValues)
        {
            return this.component.Histogram(name, value, labelValues);
        }

        /// <inheritdoc />
        public virtual Task Histogram(Enum enumValue, double value, params string[] labelValues)
        {
            return this.component.Histogram(enumValue, value, labelValues);
        }

        /// <inheritdoc />
        public virtual Task Histogram(MetricInfo metricInfo, double value)
        {
            return this.component.Histogram(metricInfo, value);
        }

        /// <inheritdoc />
        public virtual Task ResetMetrics()
        {
            return this.component.ResetMetrics();
        }

        /// <inheritdoc />
        public virtual TimeMetric Time(string name, string description, params string[] labelValues)
        {
            return this.component.Time(name, description, labelValues);
        }

        /// <inheritdoc />
        public virtual TimeMetric Time(string name, params string[] labelValues)
        {
            return this.component.Time(name, labelValues);
        }

        /// <inheritdoc />
        public virtual TimeMetric Time(Enum enumValue, params string[] labelValues)
        {
            return this.component.Time(enumValue, labelValues);
        }

        /// <inheritdoc />
        public virtual TimeMetric Time(MetricInfo metricInfo)
        {
            return this.component.Time(metricInfo);
        }
    }
}
