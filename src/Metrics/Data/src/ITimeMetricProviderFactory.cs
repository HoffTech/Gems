// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Metrics.Contracts;

namespace Gems.Metrics.Data;

public interface ITimeMetricProviderFactory
{
    TimeMetricProvider Create(MetricInfo? dbMetricInfo);
}
