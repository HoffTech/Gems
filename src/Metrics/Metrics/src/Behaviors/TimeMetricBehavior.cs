// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Metrics.LabelsProvider;
using Gems.Utils;

using MediatR;

namespace Gems.Metrics.Behaviors
{
    public class TimeMetricBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseRequest
    {
        private readonly IMetricsService metricsService;
        private readonly LabelsProviderSelector labelsProviderSelector;

        public TimeMetricBehavior(IMetricsService metricsService, LabelsProviderSelector labelsProviderSelector)
        {
            this.metricsService = metricsService;
            this.labelsProviderSelector = labelsProviderSelector;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            await using var timeMetric = request is IRequestTimeMetric ext && ext.GetTimeMetricType() != null
                ? this.metricsService.Time(ext.GetTimeMetricType(), this.GetTimeMetricLabels(request))
                : this.metricsService.Time(GetTimeMetricName());
            return await next().ConfigureAwait(false);
        }

        private static string GetTimeMetricName()
        {
            var friendlyName = typeof(TRequest).Name;
            friendlyName = friendlyName.Replace("Command", "Time");
            friendlyName = friendlyName.Replace("Query", "Time");
            friendlyName = StringUtils.ToFriendlyName(friendlyName);
            return StringUtils.MapSpaceToUndescore(friendlyName.ToLower());
        }

        private string[] GetTimeMetricLabels(TRequest request)
        {
            return this.labelsProviderSelector.GetLabelsProvider<TRequest>()?.GetLabels(request) ?? Array.Empty<string>();
        }
    }
}
