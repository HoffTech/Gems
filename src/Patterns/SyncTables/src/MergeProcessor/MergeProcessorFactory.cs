// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoMapper;

using Gems.Patterns.SyncTables.EntitiesViews;
using Gems.Patterns.SyncTables.MergeProcessor.MergeInfos;

namespace Gems.Patterns.SyncTables.MergeProcessor
{
    public class MergeProcessorFactory
    {
        private readonly ExternalEntitiesProvider externalEntitiesProvider;
        private readonly EntitiesUpdater entitiesUpdater;
        private readonly IMapper mapper;

        public MergeProcessorFactory(
            ExternalEntitiesProvider externalEntitiesProvider,
            EntitiesUpdater entitiesUpdater,
            IMapper mapper)
        {
            this.externalEntitiesProvider = externalEntitiesProvider;
            this.entitiesUpdater = entitiesUpdater;
            this.mapper = mapper;
        }

        public MergeProcessor<TExternalEntity, TTargetEntity, TMergeResult>
            CreateMergeProcessor<TExternalEntity, TTargetEntity, TMergeResult>(MergeInfo<TMergeResult> mergeInfo)
            where TExternalEntity : ExternalEntity
            where TTargetEntity : class
            where TMergeResult : class, new()
        {
            return new MergeProcessor<TExternalEntity, TTargetEntity, TMergeResult>(
                mergeInfo,
                this.externalEntitiesProvider,
                this.entitiesUpdater,
                this.mapper);
        }
    }
}
