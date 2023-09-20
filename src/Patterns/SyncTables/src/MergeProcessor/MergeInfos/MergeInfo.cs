// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Gems.Patterns.SyncTables.EntitiesViews;

namespace Gems.Patterns.SyncTables.MergeProcessor.MergeInfos
{
    public class MergeInfo<TMergeResult>
        where TMergeResult : class
    {
        public MergeInfo(
            string sourceDbKey,
            string externalSyncQuery,
            string mergeFunctionName,
            string mergeParameterName,
            bool needConvertDateTimeToUtc,
            int getCommandTimeout = 30,
            string targetDbKey = "default")
        {
            this.SourceDbKey = sourceDbKey;
            this.TargetDbKey = targetDbKey;
            this.ExternalSyncQuery = externalSyncQuery;
            this.MergeFunctionName = mergeFunctionName;
            this.MergeParameterName = mergeParameterName;
            this.NeedConvertDateTimeToUtc = needConvertDateTimeToUtc;
            this.GetCommandTimeout = getCommandTimeout;
        }

        public string SourceDbKey { get; }

        public string TargetDbKey { get; }

        public string ExternalSyncQuery { get; }

        public string MergeFunctionName { get; }

        public string MergeParameterName { get; }

        public bool NeedConvertDateTimeToUtc { get; }

        public int GetCommandTimeout { get; }

        public List<ExternalEntity> ExternalEntities { get; set; }

        public TMergeResult MergeResult { get; set; }
    }
}
