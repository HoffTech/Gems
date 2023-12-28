// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Metrics.Contracts;
using Gems.Metrics.Prometheus.Contracts;

using PrometheusThirdParty = Prometheus;

namespace Gems.Metrics.Prometheus
{
    public class PrometheusMetricsFactory
    {
        private readonly Configuration configuration;

        public PrometheusMetricsFactory(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public PrometheusThirdParty.IGauge CreateGauge(MetricInfo metricInfo)
        {
            var caugeConfiguration = this.GetGaugeConfiguration(metricInfo);
            var metric = PrometheusThirdParty.Metrics.CreateGauge(metricInfo.Name, metricInfo.Description, caugeConfiguration);
            if (metricInfo.LabelValues.Length == 0)
            {
                return metric;
            }

            return metric.WithLabels(metricInfo.LabelValues);
        }

        public PrometheusThirdParty.IGauge CreateGauge(string name, string description, params string[] labelValues)
        {
            return this.CreateGauge(new MetricInfo
            {
                Name = name,
                Description = description,
                LabelNames = Array.Empty<string>(),
                LabelValues = labelValues
            });
        }

        public PrometheusThirdParty.IGauge CreateGauge(string name, params string[] labelValues)
        {
            return this.CreateGauge(new MetricInfo
            {
                Name = name,
                Description = MetricNameHelper.GetDescription(name),
                LabelNames = Array.Empty<string>(),
                LabelValues = labelValues
            });
        }

        public PrometheusThirdParty.IGauge CreateGauge(Enum enumValue, params string[] labelValues)
        {
            var metricInfo = MetricNameHelper.GetMetricInfo(enumValue, labelValues);
            return this.CreateGauge(metricInfo);
        }

        public PrometheusThirdParty.ICounter CreateCounter(MetricInfo metricInfo)
        {
            var counterConfiguration = this.GetCounterConfiguration(metricInfo);

            var metric = PrometheusThirdParty.Metrics.CreateCounter(metricInfo.Name, metricInfo.Description, counterConfiguration);
            if (metricInfo.LabelValues.Length == 0)
            {
                return metric;
            }

            return metric.WithLabels(metricInfo.LabelValues);
        }

        public PrometheusThirdParty.ICounter CreateCounter(string name, string description, params string[] labelValues)
        {
            return this.CreateCounter(new MetricInfo
            {
                Name = name,
                Description = description,
                LabelNames = Array.Empty<string>(),
                LabelValues = labelValues
            });
        }

        public PrometheusThirdParty.ICounter CreateCounter(string name, params string[] labelValues)
        {
            return this.CreateCounter(new MetricInfo
            {
                Name = name,
                Description = MetricNameHelper.GetDescription(name),
                LabelNames = Array.Empty<string>(),
                LabelValues = labelValues
            });
        }

        public PrometheusThirdParty.ICounter CreateCounter(Enum enumValue, params string[] labelValues)
        {
            var metricInfo = MetricNameHelper.GetMetricInfo(enumValue, labelValues);
            return this.CreateCounter(metricInfo);
        }

        public PrometheusThirdParty.IHistogram CreateHistogram(MetricInfo metricInfo)
        {
            var histogramConfiguration = this.GetHistogramConfiguration(metricInfo);

            var metric = PrometheusThirdParty.Metrics.CreateHistogram(metricInfo.Name, metricInfo.Description, histogramConfiguration);
            if (metricInfo.LabelValues.Length == 0)
            {
                return metric;
            }

            return metric.WithLabels(metricInfo.LabelValues);
        }

        public PrometheusThirdParty.IHistogram CreateHistogram(string name, string description, params string[] labelValues)
        {
            return this.CreateHistogram(new MetricInfo
            {
                Name = name,
                Description = description,
                LabelNames = Array.Empty<string>(),
                LabelValues = labelValues
            });
        }

        public PrometheusThirdParty.IHistogram CreateHistogram(string name, params string[] labelValues)
        {
            return this.CreateHistogram(new MetricInfo
            {
                Name = name,
                Description = MetricNameHelper.GetDescription(name),
                LabelNames = Array.Empty<string>(),
                LabelValues = labelValues
            });
        }

        public PrometheusThirdParty.IHistogram CreateHistogram(Enum enumValue, params string[] labelValues)
        {
            var metricInfo = MetricNameHelper.GetMetricInfo(enumValue, labelValues);
            return this.CreateHistogram(metricInfo);
        }

        private PrometheusThirdParty.GaugeConfiguration GetGaugeConfiguration(MetricInfo metricInfo)
        {
            PrometheusThirdParty.GaugeConfiguration gaugeConfiguration;
            if (this.configuration.GaugeConfiguration != null
                && this.configuration.GaugeConfiguration.TryGetValue(metricInfo.Name, out var config))
            {
                gaugeConfiguration = new PrometheusThirdParty.GaugeConfiguration
                {
                    LabelNames = config.LabelNames,
                    StaticLabels = config.StaticLabels,
                    SuppressInitialValue = config.SuppressInitialValue
                };
            }
            else
            {
                gaugeConfiguration = new PrometheusThirdParty.GaugeConfiguration();
            }

            if (metricInfo.LabelNames.Length > 0)
            {
                gaugeConfiguration.LabelNames = metricInfo.LabelNames;
            }

            return gaugeConfiguration;
        }

        private PrometheusThirdParty.CounterConfiguration GetCounterConfiguration(MetricInfo metricInfo)
        {
            PrometheusThirdParty.CounterConfiguration counterConfiguration;
            if (this.configuration?.CounterConfiguration != null
                && this.configuration.CounterConfiguration.TryGetValue(metricInfo.Name, out var config))
            {
                counterConfiguration = new PrometheusThirdParty.CounterConfiguration
                {
                    LabelNames = config.LabelNames,
                    StaticLabels = config.StaticLabels,
                    SuppressInitialValue = config.SuppressInitialValue
                };
            }
            else
            {
                counterConfiguration = new PrometheusThirdParty.CounterConfiguration();
            }

            if (metricInfo.LabelNames.Length > 0)
            {
                counterConfiguration.LabelNames = metricInfo.LabelNames;
            }

            return counterConfiguration;
        }

        private PrometheusThirdParty.HistogramConfiguration GetHistogramConfiguration(MetricInfo metricInfo)
        {
            PrometheusThirdParty.HistogramConfiguration histogramConfiguration;
            if (this.configuration?.HistogramConfiguration != null
                && this.configuration.HistogramConfiguration.TryGetValue(metricInfo.Name, out var config))
            {
                histogramConfiguration = new PrometheusThirdParty.HistogramConfiguration
                {
                    LabelNames = config.LabelNames,
                    StaticLabels = config.StaticLabels,
                    SuppressInitialValue = config.SuppressInitialValue,
                    Buckets = config.Buckets
                };
            }
            else
            {
                histogramConfiguration = new PrometheusThirdParty.HistogramConfiguration();
            }

            if (metricInfo.LabelNames.Length > 0)
            {
                histogramConfiguration.LabelNames = metricInfo.LabelNames;
            }

            return histogramConfiguration;
        }
    }
}
