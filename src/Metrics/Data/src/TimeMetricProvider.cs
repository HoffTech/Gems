// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Metrics.Contracts;

namespace Gems.Metrics.Data;

public class TimeMetricProvider
{
    private const string DbQueryTimeMetricName = "db_query_time";
    private const string DbQueryTimeMetricDescription = "Db Query Time";
    private readonly IMetricsService metricsService;
    private readonly MetricInfo? dbMetricInfo;

    public TimeMetricProvider(IMetricsService metricsService, MetricInfo? dbMetricInfo)
    {
        this.metricsService = metricsService;
        this.dbMetricInfo = dbMetricInfo;
    }

    public TimeMetric GetTimeMetric(Enum timeMetricType, string functionName)
    {
        if (timeMetricType != null)
        {
            var metricInfo = MetricNameHelper.GetMetricInfo(timeMetricType);
            if (metricInfo.LabelValues.Length == 1)
            {
                metricInfo.LabelNames = metricInfo.LabelNames.Length == 1 ? metricInfo.LabelNames : new[] { "functionName" };
            }

            return this.metricsService.Time(metricInfo);
        }

        if (functionName != null)
        {
            var metricInfo = this.dbMetricInfo ?? new MetricInfo { Name = DbQueryTimeMetricName, Description = DbQueryTimeMetricDescription };
            metricInfo.LabelNames = metricInfo.LabelNames?.Length == 1 ? metricInfo.LabelNames : new[] { "functionName" };
            metricInfo.LabelNames = metricInfo.LabelNames?.Length == 1 ? metricInfo.LabelNames : new[] { "functionName" };
            metricInfo.LabelValues = new[] { GetDbCommandName(functionName) };
            return this.metricsService.Time(metricInfo);
        }

        return null;
    }

    private static string GetDbCommandName(string commandName)
    {
        return commandName.Replace(".", "_");
    }
}
