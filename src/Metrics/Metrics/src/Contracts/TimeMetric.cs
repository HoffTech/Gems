// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Gems.Utils;

namespace Gems.Metrics.Contracts
{
    public class TimeMetric : IAsyncDisposable
    {
        private readonly IMetricsService metricsService;
        private readonly TimeUnit timeUnit;
        private readonly Stopwatch stopWatch;
        private readonly MetricInfo metricInfo;

        public TimeMetric(MetricInfo metricInfo, IMetricsService metricsService, TimeUnit timeUnit = TimeUnit.Milliseconds)
            : this(metricsService, timeUnit)
        {
            this.metricInfo = metricInfo;
        }

        public TimeMetric(Enum enumValue, IMetricsService metricsService, TimeUnit timeUnit = TimeUnit.Milliseconds, params string[] labelValues)
            : this(metricsService, timeUnit)
        {
            this.metricInfo = MetricNameHelper.GetMetricInfo(enumValue, labelValues);
        }

        public TimeMetric(string name, IMetricsService metricsService, TimeUnit timeUnit = TimeUnit.Milliseconds, params string[] labelValues)
            : this(metricsService, timeUnit)
        {
            this.metricInfo = new MetricInfo
            {
                Name = name,
                Description = StringUtils.MapUndescoreToSpace(name),
                LabelValues = labelValues.Length > 0 ? labelValues : Array.Empty<string>(),
                LabelNames = Array.Empty<string>()
            };
        }

        public TimeMetric(string name, string description, IMetricsService metricsService, TimeUnit timeUnit = TimeUnit.Milliseconds, params string[] labelValues)
            : this(metricsService, timeUnit)
        {
            this.metricInfo = new MetricInfo
            {
                Name = name,
                Description = description,
                LabelValues = labelValues.Length > 0 ? labelValues : Array.Empty<string>(),
                LabelNames = Array.Empty<string>()
            };
        }

        private TimeMetric(IMetricsService metricsService, TimeUnit timeUnit = TimeUnit.Seconds)
        {
            this.metricsService = metricsService;
            this.timeUnit = timeUnit;
            this.stopWatch = new Stopwatch();
            this.stopWatch.Start();
        }

        public ValueTask DisposeAsync()
        {
            this.stopWatch.Stop();
            var value = this.timeUnit == TimeUnit.Seconds
                ? this.stopWatch.Elapsed.TotalSeconds
                : this.stopWatch.Elapsed.TotalMilliseconds;
            return new ValueTask(this.SetValue(value));
        }

        public Task ResetAsync()
        {
            return this.SetValue(0);
        }

        private Task SetValue(double value)
        {
            return this.metricsService.GaugeSet(this.metricInfo, value);
        }
    }
}
