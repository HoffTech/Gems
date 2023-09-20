// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

using Gems.Metrics.Contracts;
using Gems.Mvc;
using Gems.Mvc.Filters.Errors;
using Gems.Mvc.Filters.Exceptions;
using Gems.Utils;

using MediatR;

using Npgsql;

namespace Gems.Metrics.Behaviors
{
    public class ErrorMetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMetricsService metricsService;
        private readonly IConverter<Exception, BusinessErrorViewModel> exceptionConverter;

        public ErrorMetricsBehavior(IMetricsService metricsService, IConverter<Exception, BusinessErrorViewModel> exceptionConverter)
        {
            this.metricsService = metricsService;
            this.exceptionConverter = exceptionConverter;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
#pragma warning disable CS0618
                await this.metricsService.Gauge(GetInputMetricName()).ConfigureAwait(false);
#pragma warning restore CS0618

                var response = await next().ConfigureAwait(false);
                await this.WriteMetricsAsync("feature_counters", "none", 200).ConfigureAwait(false);

#pragma warning disable CS0618
                await this.metricsService.Gauge(GetSuccessMetricName()).ConfigureAwait(false);
#pragma warning restore CS0618

                return response;
            }
            catch (NpgsqlException ex)
            {
                await this.WriteMetricsAsync("feature_counters", "npgsql", this.exceptionConverter.Convert(ex).StatusCode).ConfigureAwait(false);
                await this.WriteMetricsAsync("errors_counter", "npgsql", this.exceptionConverter.Convert(ex).StatusCode).ConfigureAwait(false);
                throw;
            }
            catch (SqlException ex)
            {
                await this.WriteMetricsAsync("feature_counters", "mssql", this.exceptionConverter.Convert(ex).StatusCode).ConfigureAwait(false);
                await this.WriteMetricsAsync("errors_counter", "mssql", this.exceptionConverter.Convert(ex).StatusCode).ConfigureAwait(false);
                throw;
            }
            catch (Exception ex) when (ex is ValidationException exception)
            {
                await this.WriteMetricsAsync("feature_counters", "validation", this.exceptionConverter.Convert(exception).StatusCode).ConfigureAwait(false);
                await this.WriteMetricsAsync("errors_counter", "validation", this.exceptionConverter.Convert(exception).StatusCode).ConfigureAwait(false);
                throw;
            }
            catch (Exception ex) when (ex is BusinessException exception)
            {
                await this.WriteMetricsAsync("feature_counters", "business", this.exceptionConverter.Convert(exception).StatusCode).ConfigureAwait(false);
                await this.WriteMetricsAsync("errors_counter", "business", this.exceptionConverter.Convert(exception).StatusCode).ConfigureAwait(false);
                throw;
            }
            catch (Exception ex)
            {
                await this.WriteMetricsAsync("feature_counters", "other", this.exceptionConverter.Convert(ex).StatusCode).ConfigureAwait(false);
                await this.WriteMetricsAsync("errors_counter", "other", this.exceptionConverter.Convert(ex).StatusCode).ConfigureAwait(false);
                throw;
            }
        }

        [Obsolete("Оставлено для обратной совместимости. Будет удалено с версией 7.0")]
        private static string GetInputMetricName()
        {
            var friendlyName = typeof(TRequest).Name;
            friendlyName = friendlyName.Replace("Command", "Counter");
            friendlyName = friendlyName.Replace("Query", "Counter");
            friendlyName = StringUtils.ToFriendlyName(friendlyName);
            return StringUtils.MapSpaceToUndescore(friendlyName.ToLower());
        }

        [Obsolete("Оставлено для обратной совместимости. Будет удалено с версией 7.0")]
        private static string GetSuccessMetricName()
        {
            var friendlyName = typeof(TRequest).Name;
            friendlyName = friendlyName.Replace("Command", "SuccessCounter");
            friendlyName = friendlyName.Replace("Query", "SuccessCounter");
            friendlyName = StringUtils.ToFriendlyName(friendlyName);
            return StringUtils.MapSpaceToUndescore(friendlyName.ToLower());
        }

        private static string GetFeatureName()
        {
            var friendlyName = typeof(TRequest).Name;
            friendlyName = friendlyName.Replace("Command", string.Empty);
            friendlyName = friendlyName.Replace("Query", string.Empty);
            friendlyName = StringUtils.ToFriendlyName(friendlyName);
            return StringUtils.MapSpaceToUndescore(friendlyName.ToLower());
        }

        private async Task WriteMetricsAsync(string metricName, string errorType, int? statusCode)
        {
            await this.metricsService.Gauge(new MetricInfo
            {
                Name = metricName,
                LabelNames = new[] { "feature_name", "error_type", "status_code" },
                LabelValues = new[] { GetFeatureName(), errorType, statusCode.ToString() }
            }).ConfigureAwait(false);
        }
    }
}
