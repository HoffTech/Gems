// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gems.Patterns.SyncTables.MergeProcessor
{
    public class MergeCollection<TMergeResult>
        where TMergeResult : class, new()
    {
        private readonly List<BaseMergeProcessor<TMergeResult>> mergeProcessors;

        public MergeCollection(List<BaseMergeProcessor<TMergeResult>> mergeProcessors)
        {
            this.mergeProcessors = mergeProcessors;
        }

        public async Task<List<TMergeResult>> ProcessCollectionAsync(CancellationToken cancellationToken)
        {
            foreach (var processor in this.mergeProcessors)
            {
                await processor.GetEntitiesAsync(cancellationToken).ConfigureAwait(false);
            }

            foreach (var processor in this.mergeProcessors)
            {
                await processor.MergeEntitiesAsync(cancellationToken).ConfigureAwait(false);
            }

            return this.mergeProcessors.Select(p => p.MergeInfo.MergeResult).ToList();
        }
    }
}
