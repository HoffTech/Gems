// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;

using Gems.Metrics.Contracts;
using Gems.Utils;

namespace Gems.Metrics
{
    public static class MetricNameHelper
    {
        private static readonly ConcurrentDictionary<Enum, MetricInfo> MetricInfos = new ConcurrentDictionary<Enum, MetricInfo>();

        public static (string name, string description) GetNameAndDescription(Enum enumValue)
        {
            var name = GetMetricInfo(enumValue).Name;
            var description = GetMetricInfo(enumValue).Description;
            return (name, description);
        }

        public static string GetDescription(string name)
        {
            return StringUtils.MapUndescoreToSpace(name);
        }

        public static MetricInfo GetMetricInfo(Enum enumValue, params string[] labelValues)
        {
            if (enumValue == null)
            {
                throw new ArgumentNullException();
            }

            if (!Enum.IsDefined(enumValue.GetType(), enumValue))
            {
                throw new ArgumentNullException();
            }

            if (MetricInfos.TryGetValue(enumValue, out var metricInfo))
            {
                metricInfo.LabelValues = labelValues.Length > 0 ? labelValues : metricInfo.LabelValues;
                return metricInfo;
            }

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
            {
                throw new ArgumentNullException();
            }

            if (!(fieldInfo.GetCustomAttributes(typeof(MetricAttribute), false) is MetricAttribute[] attributes) || attributes.Length <= 0)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                var friendlyName = EnumUtils.ToFriendlyName(enumValue);
                var name = StringUtils.MapSpaceToUndescore(friendlyName.ToLower());
                var description = EnumUtils.ToDescription(enumValue) ?? friendlyName;
                var labelValuesFromEnum = EnumUtils.GetLabelValues(enumValue);
#pragma warning restore CS0618 // Type or member is obsolete
                metricInfo = new MetricInfo
                {
                    Name = name,
                    Description = description,
                    LabelNames = Array.Empty<string>(),
                    LabelValues = labelValuesFromEnum ?? Array.Empty<string>()
                };
            }
            else
            {
                var friendlyName = StringUtils.ToFriendlyName(enumValue.ToString());
                metricInfo = new MetricInfo
                {
                    Name = attributes[0].Name ?? StringUtils.MapSpaceToUndescore(friendlyName.ToLower()),
                    Description = attributes[0].Description ?? friendlyName,
                    LabelNames = attributes[0].LabelNames ?? Array.Empty<string>(),
                    LabelValues = attributes[0].LabelValues ?? Array.Empty<string>()
                };
            }

            MetricInfos.TryAdd(enumValue, metricInfo);
            metricInfo.LabelValues = labelValues.Length > 0 ? labelValues : metricInfo.LabelValues;
            return metricInfo;
        }
    }
}
