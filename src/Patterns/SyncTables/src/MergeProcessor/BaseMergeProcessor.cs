// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Patterns.SyncTables.MergeProcessor.MergeInfos;

namespace Gems.Patterns.SyncTables.MergeProcessor
{
    public abstract class BaseMergeProcessor<TMergeResult>
        where TMergeResult : class
    {
        public BaseMergeProcessor(MergeInfo<TMergeResult> mergeInfo)
        {
            this.MergeInfo = mergeInfo;
        }

        public MergeInfo<TMergeResult> MergeInfo { get; }

        public abstract Task GetEntitiesAsync(CancellationToken cancellationToken);

        public abstract Task MergeEntitiesAsync(CancellationToken cancellationToken);
    }
}
