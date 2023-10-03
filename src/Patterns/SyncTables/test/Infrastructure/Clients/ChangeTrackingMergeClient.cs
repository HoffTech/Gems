// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Patterns.SyncTables.MergeProcessor;
using Gems.Patterns.SyncTables.MergeProcessor.MergeInfos;

namespace Gems.Patterns.SyncTables.Tests.Infrastructure.Clients
{
    public class ChangeTrackingMergeClient
    {
        private readonly ChangeTrackingMergeProcessorFactory processorFactory;

        public ChangeTrackingMergeClient(ChangeTrackingMergeProcessorFactory processorFactory)
        {
            this.processorFactory = processorFactory;
        }

        public async Task<List<MergeResult>> ProcessMergesAsync(
            ChangeTrackingMergeInfo<MergeResult> mergeInfo1,
            ChangeTrackingMergeInfo<MergeResult> mergeInfo2,
            CancellationToken cancellationToken)
        {
            var mergeCollection = new MergeCollection<MergeResult>(new List<BaseMergeProcessor<MergeResult>>
            {
                this.processorFactory
                    .CreateChangeTrackingMergeProcessor<RealExternalChangeTrackingEntity, RealTargetEntity, MergeResult>(
                        mergeInfo1),

                this.processorFactory
                    .CreateChangeTrackingMergeProcessor<RealExternalChangeTrackingEntity, RealTargetEntity, MergeResult>(
                        mergeInfo2)
            });

            return await mergeCollection.ProcessCollectionAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
