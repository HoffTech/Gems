// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Net;
using System.Threading.Tasks;

using Gems.Metrics.Contracts;

namespace Gems.Metrics.Http
{
    public class RequestMetricWriter
    {
        private const string StatusCodeRequestMetricName = "status_code_request";
        private readonly IMetricsService metricsService;
        private readonly string requestUri;
        private readonly Enum statusCodeMetricType;

        public RequestMetricWriter(IMetricsService metricsService, Enum statusCodeMetricType, object request, string requestUri)
        {
            this.metricsService = metricsService;
            this.statusCodeMetricType = statusCodeMetricType;

            this.requestUri = requestUri ?? string.Empty;
            if (request is IStatusCodeMetricAvailable metricsRequest)
            {
                this.statusCodeMetricType = metricsRequest.StatusCodeMetricType;
            }
        }

        public RequestMetricWriter(IMetricsService metricsService, Enum statusCodeMetricType, string requestUri = null)
        {
            this.metricsService = metricsService;
            this.statusCodeMetricType = statusCodeMetricType;

            this.requestUri = requestUri ?? string.Empty;
        }

        public Task WriteMetricsAsError200(HttpStatusCode statusCode)
        {
            return this.WriteMetrics(statusCode, "error", "error_200");
        }

        public async Task WriteMetrics(HttpStatusCode statusCode, string statusGroup = null, string statusSubGroup = null)
        {
            if (this.metricsService == null)
            {
                return;
            }

            const string successStatusGroup = "success";
            const string errorStatusGroup = "error";

            var code = (int)statusCode;

            if (string.IsNullOrEmpty(statusGroup))
            {
                statusGroup = code < 400 ? successStatusGroup : errorStatusGroup;
            }

            if (string.IsNullOrEmpty(statusSubGroup))
            {
                statusSubGroup = code < 400 ? $"{successStatusGroup}_200" :
                    (code < 500 ? $"{errorStatusGroup}_400" : $@"{errorStatusGroup}_500");
            }

            var metricInfo = this.statusCodeMetricType != null
                ? MetricNameHelper.GetMetricInfo(this.statusCodeMetricType)
                : new MetricInfo
                {
                    Name = StatusCodeRequestMetricName
                };
            metricInfo.LabelNames = new[] { "statusGroup", "statusSubGroup", "statusCode", "requestUri" };
            metricInfo.LabelValues = new[] { statusGroup, statusSubGroup, code.ToString(), this.requestUri };
            await this.metricsService.Gauge(metricInfo, 1).ConfigureAwait(false);
        }

        public TimeMetric GetTimeMetric()
        {
            if (this.metricsService == null)
            {
                return null;
            }

            var metricInfo = this.statusCodeMetricType != null
                ? MetricNameHelper.GetMetricInfo(this.statusCodeMetricType)
                : new MetricInfo
                {
                    Name = StatusCodeRequestMetricName
                };
            metricInfo.Name += "_time";
            metricInfo.LabelNames = new[] { "requestUri" };
            metricInfo.LabelValues = new[] { this.requestUri };
            return this.metricsService.Time(metricInfo);
        }
    }
}
