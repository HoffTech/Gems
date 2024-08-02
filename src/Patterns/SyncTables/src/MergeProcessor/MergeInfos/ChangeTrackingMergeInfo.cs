// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

using Gems.Patterns.SyncTables.EntitiesViews;

namespace Gems.Patterns.SyncTables.MergeProcessor.MergeInfos
{
    [Obsolete("Будет удалено в 7.0, нужно пользоваться...")]
    public class ChangeTrackingMergeInfo<TMergeResult> : MergeInfo<TMergeResult>
        where TMergeResult : class, new()
    {
        public ChangeTrackingMergeInfo(
            string sourceDbKey,
            string tableName,
            string externalSyncQuery,
            string mergeFunctionName,
            string mergeParameterName,
            bool needConvertDateTimeToUtc,
            int getCommandTimeout = 30,
            string targetDbKey = "default",
            Enum externalDbQueryMetricType = null)
            : base(
                sourceDbKey,
                externalSyncQuery,
                mergeFunctionName,
                mergeParameterName,
                needConvertDateTimeToUtc,
                getCommandTimeout,
                targetDbKey,
                externalDbQueryMetricType)
        {
            this.TableName = tableName;
        }

        public string TableName { get; }

        public long TableVersion { get; set; }

        public new List<ExternalChangeTrackingEntity> ExternalEntities { get; set; }
    }
}
