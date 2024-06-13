// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Metrics.Contracts;

namespace Gems.Data.Sample.Metrics
{
    public enum AtomicQueryMetricType
    {
        [Metric(
            Name = "gems_data_sample_atomic_metrics_db_query",
            Description = "Time of Atomic Gems Data Sample Metrics db query")]
        GemsDataSampleMetricsAtomicDbQueryTime
    }
}
