// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using AutoMapper;

using Gems.Patterns.SyncTables.EntitiesViews;
using Gems.Patterns.SyncTables.MergeProcessor.MergeInfos;

namespace Gems.Patterns.SyncTables.MergeProcessor
{
    [Obsolete("Будет удалено в 7.0, необходимо перейти на использование ChangeTrackingSyncTableProcessor через DI")]
    public class ChangeTrackingMergeProcessorFactory
    {
        private readonly RowVersionProvider rowVersionProvider;
        private readonly RowVersionUpdater rowVersionUpdater;
        private readonly ExternalEntitiesProvider externalEntitiesProvider;
        private readonly EntitiesUpdater entitiesUpdater;
        private readonly IMapper mapper;

        public ChangeTrackingMergeProcessorFactory(
            RowVersionProvider rowVersionProvider,
            RowVersionUpdater rowVersionUpdater,
            ExternalEntitiesProvider externalEntitiesProvider,
            EntitiesUpdater entitiesUpdater,
            IMapper mapper)
        {
            this.rowVersionProvider = rowVersionProvider;
            this.rowVersionUpdater = rowVersionUpdater;
            this.externalEntitiesProvider = externalEntitiesProvider;
            this.entitiesUpdater = entitiesUpdater;
            this.mapper = mapper;
        }

        public ChangeTrackingMergeProcessor<TExternalEntity, TTargetEntity, TMergeResult>
            CreateChangeTrackingMergeProcessor<TExternalEntity, TTargetEntity, TMergeResult>(ChangeTrackingMergeInfo<TMergeResult> mergeInfo)
            where TExternalEntity : ExternalEntity
            where TTargetEntity : class
            where TMergeResult : class, new()
        {
            return new ChangeTrackingMergeProcessor<TExternalEntity, TTargetEntity, TMergeResult>(
                mergeInfo,
                this.rowVersionProvider,
                this.rowVersionUpdater,
                this.externalEntitiesProvider,
                this.entitiesUpdater,
                this.mapper);
        }
    }
}
