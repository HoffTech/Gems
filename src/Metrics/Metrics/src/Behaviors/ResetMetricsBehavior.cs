// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Metrics.Contracts;
using Gems.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.Metrics.Behaviors
{
    public class ResetMetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMetricsService metricsService;
        private readonly IOptions<MetricsConfig> metricsConfig;
        private readonly ILogger<ResetMetricsBehavior<TRequest, TResponse>> logger;

        public ResetMetricsBehavior(IMetricsService metricsService, IOptions<MetricsConfig> metricsConfig, ILogger<ResetMetricsBehavior<TRequest, TResponse>> logger)
        {
            this.metricsService = metricsService;
            this.metricsConfig = metricsConfig;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                var response = await next().ConfigureAwait(false);
                return response;
            }
            finally
            {
                this.ResetMetricsAsync().SafeFireAndForget(false, exception => this.logger.LogError(exception, "Failed reset metrics"));
            }
        }

        private async Task ResetMetricsAsync()
        {
            if (this.metricsConfig?.Value?.ResetMillisecondsDelay == 0)
            {
                throw new ArgumentNullException(
                    $"You must specify ResetMillisecondsDelay in MetricsConfig. See documentation of Gems.Metrics.Abstractions library.");
            }

            await Task.Delay(this.metricsConfig!.Value!.ResetMillisecondsDelay).ConfigureAwait(false);
            await this.metricsService.ResetMetrics().ConfigureAwait(false);
        }
    }
}
