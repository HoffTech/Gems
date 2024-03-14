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

#pragma warning disable CS0618
                await this.metricsService.Gauge(GetSuccessMetricName()).ConfigureAwait(false);
#pragma warning restore CS0618

                await this.WriteMetricsAsync().ConfigureAwait(false);
                return response;
            }
            catch (NpgsqlException ex)
            {
                await this.WriteMetricsAsync("npgsql", ex).ConfigureAwait(false);
                throw;
            }
            catch (SqlException ex)
            {
                await this.WriteMetricsAsync("mssql", ex).ConfigureAwait(false);
                throw;
            }
            catch (Exception ex) when (ex is FluentValidation.ValidationException or ValidationException)
            {
                await this.WriteMetricsAsync("validation", ex).ConfigureAwait(false);
                throw;
            }
            catch (Exception ex) when (ex is BusinessException exception)
            {
                await this.WriteMetricsAsync("business", ex).ConfigureAwait(false);
                throw;
            }
            catch (Exception ex)
            {
                await this.WriteMetricsAsync("other", ex).ConfigureAwait(false);
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

        private async Task WriteMetricsAsync(string errorType = null, Exception exception = null)
        {
            errorType ??= "none";
            var featureName = GetFeatureName();
            var statusCode = "200";
            var customCode = "none";
            if (exception != null)
            {
                var businessErrorViewModel = this.exceptionConverter.Convert(exception);
                statusCode = businessErrorViewModel.StatusCode.ToString();
                customCode = businessErrorViewModel.Error?.Code ?? "none";
            }

            await this.metricsService.Gauge(new MetricInfo
            {
                Name = "feature_counters",
                LabelNames = new[] { "feature_name", "error_type", "status_code", "custom_code" },
                LabelValues = new[] { featureName, errorType, statusCode, customCode }
            }).ConfigureAwait(false);

            if (errorType == "none")
            {
                return;
            }

            await this.metricsService.Gauge(new MetricInfo
            {
                Name = "errors_counter",
                LabelNames = new[] { "feature_name", "error_type", "status_code", "custom_code" },
                LabelValues = new[] { featureName, errorType, statusCode, customCode }
            }).ConfigureAwait(false);
        }
    }
}
