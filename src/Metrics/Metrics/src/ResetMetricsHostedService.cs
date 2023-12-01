// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Metrics.Contracts;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.Metrics
{
    public class ResetMetricsHostedService : BackgroundService
    {
        private readonly ILogger<ResetMetricsHostedService> logger;
        private readonly IOptions<MetricsConfig> options;
        private readonly IMetricsService metricsService;

        public ResetMetricsHostedService(
            ILogger<ResetMetricsHostedService> logger,
            IOptions<MetricsConfig> options,
            IMetricsService metricsService)
        {
            this.logger = logger;
            this.options = options;
            this.metricsService = metricsService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await this.metricsService.ResetMetrics().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, "Failed reset metrics.");
                }
                finally
                {
                    await Task
                        .Delay(this.options.Value.ResetMillisecondsDelay, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
