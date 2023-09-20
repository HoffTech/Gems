// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading.Tasks;

using Gems.Metrics.Contracts;

namespace Gems.Metrics;

public static class TimeMetricExtensions
{
    public static async Task DisposeMetric(this TimeMetric metric)
    {
        if (metric != null)
        {
            await metric.DisposeAsync().ConfigureAwait(false);
        }
    }
}
