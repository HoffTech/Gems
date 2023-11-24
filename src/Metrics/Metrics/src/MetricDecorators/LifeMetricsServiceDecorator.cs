// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Gems.Metrics.Contracts;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.Metrics.MetricDecorators
{
    public class LifeMetricsServiceDecorator : AMetricsServiceDecorator
    {
        private const int DefaultResetMillisecondsDelay = 60000;

        private readonly ConcurrentDictionary<MetricInfo, MetricLifeInfo> gaugeMetrics = new ConcurrentDictionary<MetricInfo, MetricLifeInfo>();
        private readonly ILogger<IMetricsService> logger;
        private readonly IOptions<MetricsConfig> metricsConfig;

        public LifeMetricsServiceDecorator(
            ILogger<IMetricsService> logger,
            IOptions<MetricsConfig> metricsConfig,
            IHostApplicationLifetime hostLifetime)
        {
            this.logger = logger;
            this.metricsConfig = metricsConfig;
            this.SubscribeOnApplicationStopping(hostLifetime);
        }

        /// <inheritdoc />
        public override Task Gauge(string name, string description, double increment = 1, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(name, description, labelValues);
            return this.component.Gauge(name, description, increment, labelValues);
        }

        /// <inheritdoc />
        public override Task Gauge(string name, double increment = 1, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(name, labelValues);
            return this.component.Gauge(name, increment, labelValues);
        }

        /// <inheritdoc />
        public override Task Gauge(Enum enumValue, double increment = 1, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(enumValue, labelValues);
            return this.component.Gauge(enumValue, increment, labelValues);
        }

        /// <inheritdoc />
        public override Task Gauge(MetricInfo metricInfo, double increment = 1)
        {
            this.AddOrUpdateGaugeMetric(metricInfo);
            return this.component.Gauge(metricInfo, increment);
        }

        /// <inheritdoc />
        public override Task GaugeDec(string name, string description, double decrement = 1, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(name, description, labelValues);
            return this.component.GaugeDec(name, description, decrement, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeDec(string name, double decrement = 1, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(name, labelValues);
            return this.component.GaugeDec(name, decrement, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeDec(Enum enumValue, double decrement = 1, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(enumValue, labelValues);
            return this.component.GaugeDec(enumValue, decrement, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeDec(MetricInfo metricInfo, double decrement = 1)
        {
            this.AddOrUpdateGaugeMetric(metricInfo);
            return this.component.GaugeDec(metricInfo, decrement);
        }

        /// <inheritdoc />
        public override Task GaugeDecTo(string name, string description, double targetValue, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(name, description, labelValues);
            return this.component.GaugeDecTo(name, description, targetValue, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeDecTo(string name, double targetValue, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(name, labelValues);
            return this.component.GaugeDecTo(name, targetValue, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeDecTo(Enum enumValue, double targetValue, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(enumValue, labelValues);
            return this.component.GaugeDecTo(enumValue, targetValue, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeDecTo(MetricInfo metricInfo, double targetValue)
        {
            this.AddOrUpdateGaugeMetric(metricInfo);
            return this.component.GaugeDecTo(metricInfo, targetValue);
        }

        /// <inheritdoc />
        public override Task GaugeIncTo(string name, string description, double targetValue, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(name, description, labelValues);
            return this.component.GaugeIncTo(name, description, targetValue, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeIncTo(string name, double targetValue, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(name, labelValues);
            return this.component.GaugeIncTo(name, targetValue, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeIncTo(Enum enumValue, double targetValue, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(enumValue, labelValues);
            return this.component.GaugeIncTo(enumValue, targetValue, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeIncTo(MetricInfo metricInfo, double targetValue)
        {
            this.AddOrUpdateGaugeMetric(metricInfo);
            return this.component.GaugeIncTo(metricInfo, targetValue);
        }

        /// <inheritdoc />
        public override Task GaugeSet(string name, string description, double targetValue, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(name, description, labelValues);
            return this.component.GaugeSet(name, description, targetValue, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeSet(string name, double targetValue, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(name, labelValues);
            return this.component.GaugeSet(name, targetValue, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeSet(Enum enumValue, double targetValue, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(enumValue, labelValues);
            return this.component.GaugeSet(enumValue, targetValue, labelValues);
        }

        /// <inheritdoc />
        public override Task GaugeSet(MetricInfo metricInfo, double targetValue)
        {
            this.AddOrUpdateGaugeMetric(metricInfo);
            return this.component.GaugeSet(metricInfo, targetValue);
        }

        /// <inheritdoc />
        public override TimeMetric Time(string name, string description, params string[] labelValues)
        {
            return new TimeMetric(name, description, this, labelValues: labelValues);
        }

        /// <inheritdoc />
        public override TimeMetric Time(string name, params string[] labelValues)
        {
            return new TimeMetric(name, this, labelValues: labelValues);
        }

        /// <inheritdoc />
        public override TimeMetric Time(Enum enumValue, params string[] labelValues)
        {
            return new TimeMetric(enumValue, this, labelValues: labelValues);
        }

        /// <inheritdoc />
        public override TimeMetric Time(MetricInfo metricInfo)
        {
            return new TimeMetric(metricInfo, this);
        }

        /// <inheritdoc />
        public override Task ResetMetrics()
        {
            return this.ResetMetricsInternal(this.GetResetMillisecondsDelay());
        }

        private void AddOrUpdateGaugeMetric(string name, params string[] labelValues)
        {
            var description = MetricNameHelper.GetDescription(name);
            this.AddOrUpdateGaugeMetric(new MetricInfo
            {
                Name = name,
                Description = description,
                LabelNames = Array.Empty<string>(),
                LabelValues = labelValues
            });
        }

        private void AddOrUpdateGaugeMetric(string name, string description, params string[] labelValues)
        {
            this.AddOrUpdateGaugeMetric(new MetricInfo
            {
                Name = name,
                Description = description,
                LabelNames = Array.Empty<string>(),
                LabelValues = labelValues
            });
        }

        private void AddOrUpdateGaugeMetric(Enum enumValue, params string[] labelValues)
        {
            var metricInfo = MetricNameHelper.GetMetricInfo(enumValue, labelValues);
            this.AddOrUpdateGaugeMetric(metricInfo);
        }

        private void AddOrUpdateGaugeMetric(MetricInfo metricInfo)
        {
            this.gaugeMetrics.AddOrUpdate(
                metricInfo,
                _ => new MetricLifeInfo(metricInfo, DateTime.UtcNow),
                (_, _) => new MetricLifeInfo(metricInfo, DateTime.UtcNow));
        }

        private async Task ResetMetricsInternal(int resetMillisecondsDelay)
        {
            var dateExpired = DateTime.UtcNow.AddMilliseconds(-resetMillisecondsDelay);
            foreach (var gaugeMetric in this.gaugeMetrics)
            {
                if (dateExpired > gaugeMetric.Value.CreatedDate && this.gaugeMetrics.TryRemove(gaugeMetric.Key, out _))
                {
                    await this.component.GaugeSet(gaugeMetric.Value.MapToMetricInfo(), 0)
                        .ConfigureAwait(false);
                }
            }
        }

        private void SubscribeOnApplicationStopping(IHostApplicationLifetime hostLifetime)
        {
            hostLifetime.ApplicationStopping.Register(async () =>
            {
                this.logger.LogInformation("Reset metrics starting on stopping event..");
                await this.ResetMetricsInternal(-this.GetResetMillisecondsDelay()).ConfigureAwait(false); // it should be any negative number for all metrics to expire to force reset them
                await Task.Delay(this.GetResetMillisecondsDelay()).ConfigureAwait(false);
            });
        }

        private int GetResetMillisecondsDelay()
        {
            return this.metricsConfig?.Value?.ResetMillisecondsDelay ?? DefaultResetMillisecondsDelay;
        }
    }
}
