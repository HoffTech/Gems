// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

using Gems.Metrics.Contracts;
using Gems.Metrics.Prometheus.Contracts;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.Metrics.Prometheus
{
    /// <summary>
    /// Сервис для работы с метриками Prometheus.
    /// </summary>
    public class MetricsService : IMetricsService
    {
        private readonly IOptions<PrometheusMetrics> prometheusMetrics;
        private readonly ILogger<MetricsService> logger;
        private PrometheusMetricsFactory prometheusMetricsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsService"/> class.
        /// </summary>
        /// <param name="prometheusMetrics">prometheusMetrics.</param>
        /// <param name="logger">logger.</param>
        public MetricsService(IOptions<PrometheusMetrics> prometheusMetrics, ILogger<MetricsService> logger)
        {
            this.prometheusMetrics = prometheusMetrics;
            this.logger = logger;
        }

        public PrometheusMetricsFactory PrometheusMetricsFactory => this.prometheusMetricsFactory ??= new PrometheusMetricsFactory(this
            .prometheusMetrics?.Value?.Configuration);

        /// <inheritdoc />
        public Task Counter(string name, string description, double increment = 1, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateCounter(name, description, labelValues).Inc(increment);
            }
            catch (Exception)
            {
                this.logger.LogError($"Counter failed. name: {name}, description: {description}, increment: {increment}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Counter(string name, double increment = 1, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateCounter(name, labelValues).Inc(increment);
            }
            catch (Exception)
            {
                this.logger.LogError($"Counter failed. name: {name}, increment: {increment}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Counter(Enum enumValue, double increment = 1, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateCounter(enumValue, labelValues).Inc(increment);
            }
            catch (Exception)
            {
                this.logger.LogError($"Counter failed. enum: {enumValue}, increment: {increment}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Counter(MetricInfo metricInfo, double increment = 1)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateCounter(metricInfo).Inc(increment);
            }
            catch (Exception)
            {
                this.logger.LogError($"Counter failed. metricInfo: {metricInfo}, increment: {increment}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task CounterTo(string name, string description, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateCounter(name, description, labelValues).IncTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, description: {description}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task CounterTo(string name, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateCounter(name, labelValues).IncTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task CounterTo(Enum enumValue, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateCounter(enumValue, labelValues).IncTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. enum: {enumValue}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task CounterTo(MetricInfo metricInfo, double targetValue)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateCounter(metricInfo).IncTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. metricInfo: {metricInfo}, targetValue: {targetValue}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Gauge(string name, string description, double increment = 1, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(name, description, labelValues).Inc(increment);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, description: {description}, increment: {increment}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Gauge(string name, double increment = 1, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(name, labelValues).Inc(increment);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                this.logger.LogError($"CounterTo failed. name: {name}, increment: {increment}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Gauge(Enum enumValue, double increment = 1, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(enumValue, labelValues).Inc(increment);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. enum: {enumValue}, increment: {increment}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Gauge(MetricInfo metricInfo, double increment = 1)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(metricInfo).Inc(increment);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. metricInfo: {metricInfo}, increment: {increment}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeDec(string name, string description, double decrement = 1, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(name, description, labelValues).Dec(decrement);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, description: {description}, decrement: {decrement}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeDec(string name, double decrement = 1, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(name, labelValues).Dec(decrement);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, decrement: {decrement}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeDec(Enum enumValue, double decrement = 1, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(enumValue, labelValues).Dec(decrement);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. enum: {enumValue}, decrement: {decrement}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeDec(MetricInfo metricInfo, double decrement = 1)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(metricInfo).Dec(decrement);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. metricInfo: {metricInfo}, decrement: {decrement}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeDecTo(string name, string description, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(name, description, labelValues).DecTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, description: {description}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeDecTo(string name, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(name, labelValues).DecTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeDecTo(Enum enumValue, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(enumValue, labelValues).DecTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. enum: {enumValue}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeDecTo(MetricInfo metricInfo, double targetValue)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(metricInfo).DecTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. metricInfo: {metricInfo}, targetValue: {targetValue}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeIncTo(string name, string description, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(name, description, labelValues).IncTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, description: {description}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeIncTo(string name, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(name, labelValues).IncTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeIncTo(Enum enumVaue, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(enumVaue, labelValues).IncTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. enum: {enumVaue}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeIncTo(MetricInfo metricInfo, double targetValue)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(metricInfo).IncTo(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. metricInfo: {metricInfo}, targetValue: {targetValue}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeSet(string name, string description, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(name, description, labelValues).Set(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, description: {description}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeSet(string name, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(name, labelValues).Set(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. name: {name}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeSet(Enum enumValue, double targetValue, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(enumValue, labelValues).Set(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. enum: {enumValue}, targetValue: {targetValue}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task GaugeSet(MetricInfo metricInfo, double targetValue)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateGauge(metricInfo).Set(targetValue);
            }
            catch (Exception)
            {
                this.logger.LogError($"CounterTo failed. metricInfo: {metricInfo}, targetValue: {targetValue}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Histogram(string name, string description, double value, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateHistogram(name, description, labelValues).Observe(value);
            }
            catch (Exception)
            {
                this.logger.LogError($"Histogram failed. name: {name}, description: {description}, value: {value}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Histogram(string name, double value, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateHistogram(name, labelValues).Observe(value);
            }
            catch (Exception)
            {
                this.logger.LogError($"Histogram failed. name: {name}, value: {value}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Histogram(Enum enumValue, double value, params string[] labelValues)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateHistogram(enumValue, labelValues).Observe(value);
            }
            catch (Exception)
            {
                this.logger.LogError($"Histogram failed. enum: {enumValue}, value: {value}, labelValues: {string.Join(',', labelValues)}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Histogram(MetricInfo metricInfo, double value)
        {
            try
            {
                this.PrometheusMetricsFactory.CreateHistogram(metricInfo).Observe(value);
            }
            catch (Exception)
            {
                this.logger.LogError($"Histogram failed. metricInfo: {metricInfo}, value: {value}.");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public TimeMetric Time(string name, string description, params string[] labelValues)
        {
            return new TimeMetric(name, description, this, labelValues: labelValues);
        }

        /// <inheritdoc />
        public TimeMetric Time(string name, params string[] labelValues)
        {
            return new TimeMetric(name, this, labelValues: labelValues);
        }

        /// <inheritdoc />
        public TimeMetric Time(Enum enumValue, params string[] labelValues)
        {
            return new TimeMetric(enumValue, this, labelValues: labelValues);
        }

        /// <inheritdoc />
        public TimeMetric Time(MetricInfo metricInfo)
        {
            return new TimeMetric(metricInfo, this);
        }

        /// <inheritdoc />
        public Task ResetMetrics()
        {
            return Task.CompletedTask;
        }
    }
}
