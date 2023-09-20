// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Metrics.Contracts;

namespace Gems.Metrics.Data;

public class TimeMetricProviderFactory : ITimeMetricProviderFactory
{
    private readonly IMetricsService metricsService;

    public TimeMetricProviderFactory(IMetricsService metricsService)
    {
        this.metricsService = metricsService;
    }

    public TimeMetricProvider Create(MetricInfo? dbMetricInfo)
    {
        return new TimeMetricProvider(this.metricsService, dbMetricInfo);
    }
}
