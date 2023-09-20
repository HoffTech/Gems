// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Patterns.SyncTables.MergeProcessor;
using Gems.Patterns.SyncTables.MergeProcessor.MergeInfos;

namespace Gems.Patterns.SyncTables.Tests.Infrastructure.Clients
{
    public class MergeClient
    {
        private readonly MergeProcessorFactory processorFactory;

        public MergeClient(MergeProcessorFactory processorFactory)
        {
            this.processorFactory = processorFactory;
        }

        public async Task<List<MergeResult>> ProcessMergesAsync(
            MergeInfo<MergeResult> mergeInfo1,
            MergeInfo<MergeResult> mergeInfo2,
            MergeInfo<MergeResult> mergeInfo3,
            CancellationToken cancellationToken)
        {
            var mergeCollection = new MergeCollection<MergeResult>(new List<BaseMergeProcessor<MergeResult>>
            {
                this.processorFactory.CreateMergeProcessor<RealExternalEntity, RealTargetEntity, MergeResult>(mergeInfo1),
                this.processorFactory.CreateMergeProcessor<RealExternalEntity, RealTargetEntity, MergeResult>(mergeInfo2),
                this.processorFactory.CreateMergeProcessor<RealExternalEntity, RealTargetEntity, MergeResult>(mergeInfo3)
            });

            return await mergeCollection.ProcessCollectionAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
