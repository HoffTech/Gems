// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Linq;
using Gems.Patterns.SyncTables.EntitiesViews;
using Gems.Patterns.SyncTables.MergeProcessor.MergeInfos;
using Gems.Utils;

namespace Gems.Patterns.SyncTables.MergeProcessor
{
    public class ChangeTrackingMergeProcessor<TExternalEntity, TTargetEntity, TMergeResult> : BaseMergeProcessor<TMergeResult>
        where TExternalEntity : ExternalEntity
        where TTargetEntity : class
        where TMergeResult : class, new()
    {
        private readonly RowVersionProvider rowVersionProvider;
        private readonly RowVersionUpdater rowVersionUpdater;
        private readonly ExternalEntitiesProvider externalEntitiesProvider;
        private readonly EntitiesUpdater entitiesUpdater;
        private readonly IMapper mapper;

        public ChangeTrackingMergeProcessor(
            MergeInfo<TMergeResult> mergeInfo,
            RowVersionProvider rowVersionProvider,
            RowVersionUpdater rowVersionUpdater,
            ExternalEntitiesProvider externalEntitiesProvider,
            EntitiesUpdater entitiesUpdater,
            IMapper mapper) : base(mergeInfo)
        {
            this.rowVersionProvider = rowVersionProvider;
            this.rowVersionUpdater = rowVersionUpdater;
            this.externalEntitiesProvider = externalEntitiesProvider;
            this.entitiesUpdater = entitiesUpdater;
            this.mapper = mapper;
        }

        public override async Task GetEntitiesAsync(CancellationToken cancellationToken)
        {
            var mergeInfo = (ChangeTrackingMergeInfo<TMergeResult>)this.MergeInfo;

            var version = await this.rowVersionProvider
                .GetLastRowVersionForTableAsync(
                    mergeInfo.TargetDbKey,
                    mergeInfo.TableName,
                    cancellationToken)
                .ConfigureAwait(false);

            var externalEntities = await this.externalEntitiesProvider
                .GetExternalEntitiesByQueryWithVersionAsync<TExternalEntity>(
                    mergeInfo.SourceDbKey,
                    version,
                    mergeInfo.ExternalSyncQuery,
                    mergeInfo.GetCommandTimeout,
                    mergeInfo.ExternalDbQueryMetricType,
                    cancellationToken)
                .ConfigureAwait(false);

            if (externalEntities.IsNullOrEmpty())
            {
                return;
            }

            if (mergeInfo.NeedConvertDateTimeToUtc)
            {
                externalEntities.ForEach(DateTimeUtils.SetUnspecifiedToUtcDateTime);
            }

            mergeInfo.ExternalEntities = externalEntities.Cast<ExternalChangeTrackingEntity>().ToList();
            mergeInfo.TableVersion = mergeInfo.ExternalEntities[^1].RowVersion;
        }

        public override async Task MergeEntitiesAsync(CancellationToken cancellationToken)
        {
            var mergeInfo = (ChangeTrackingMergeInfo<TMergeResult>)this.MergeInfo;

            if (mergeInfo.ExternalEntities.IsNullOrEmpty())
            {
                mergeInfo.MergeResult = new TMergeResult();
                return;
            }

            var entities = this.mapper.Map<List<TTargetEntity>>(mergeInfo.ExternalEntities);
            this.MergeInfo.ExternalEntities = null;
            mergeInfo.ExternalEntities = null;

            var counters = await this.entitiesUpdater
                .MergeEntitiesAsync<TTargetEntity, TMergeResult>(
                    mergeInfo.TargetDbKey,
                    entities,
                    mergeInfo.MergeFunctionName,
                    mergeInfo.MergeParameterName,
                    cancellationToken)
                .ConfigureAwait(false);

            if (counters is ITargetEntitiesHolder targetEntitiesHolder)
            {
                targetEntitiesHolder.Entities = entities;
            }

            await this.rowVersionUpdater
                .UpsertRowVersionByTableNameAsync(
                    mergeInfo.TargetDbKey,
                    mergeInfo.TableName,
                    mergeInfo.TableVersion,
                    cancellationToken)
                .ConfigureAwait(false);

            mergeInfo.MergeResult = counters;
        }
    }
}
