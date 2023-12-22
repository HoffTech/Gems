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
    public class MergeProcessor<TExternalEntity, TTargetEntity, TMergeResult> : BaseMergeProcessor<TMergeResult>
        where TExternalEntity : class
        where TTargetEntity : class
        where TMergeResult : class, new()
    {
        private readonly ExternalEntitiesProvider externalEntitiesProvider;
        private readonly EntitiesUpdater entitiesUpdater;
        private readonly IMapper mapper;

        public MergeProcessor(
            MergeInfo<TMergeResult> mergeInfo,
            ExternalEntitiesProvider externalEntitiesProvider,
            EntitiesUpdater entitiesUpdater,
            IMapper mapper) : base(mergeInfo)
        {
            this.externalEntitiesProvider = externalEntitiesProvider;
            this.entitiesUpdater = entitiesUpdater;
            this.mapper = mapper;
        }

        public override async Task GetEntitiesAsync(CancellationToken cancellationToken)
        {
            var externalEntities = await this.externalEntitiesProvider
                .GetExternalEntitiesByQueryAsync<TExternalEntity>(
                    this.MergeInfo.SourceDbKey,
                    this.MergeInfo.ExternalSyncQuery,
                    this.MergeInfo.GetCommandTimeout,
                    this.MergeInfo.ExternalDbQueryMetricType,
                    cancellationToken)
                .ConfigureAwait(false);

            if (externalEntities.IsNullOrEmpty())
            {
                return;
            }

            if (this.MergeInfo.NeedConvertDateTimeToUtc)
            {
                DateTimeUtils.SetUnspecifiedToUtcDateTime(externalEntities);
            }

            this.MergeInfo.ExternalEntities = externalEntities.Cast<ExternalEntity>().ToList();
        }

        public override async Task MergeEntitiesAsync(CancellationToken cancellationToken)
        {
            if (this.MergeInfo.ExternalEntities.IsNullOrEmpty())
            {
                this.MergeInfo.MergeResult = new TMergeResult();
                return;
            }

            var entities = this.mapper.Map<List<TTargetEntity>>(this.MergeInfo.ExternalEntities);
            this.MergeInfo.ExternalEntities = null;

            var mergeResult = await this.entitiesUpdater
                .MergeEntitiesAsync<TTargetEntity, TMergeResult>(
                    this.MergeInfo.TargetDbKey,
                    entities,
                    this.MergeInfo.MergeFunctionName,
                    this.MergeInfo.MergeParameterName,
                    cancellationToken)
                .ConfigureAwait(false);

            this.MergeInfo.MergeResult = mergeResult;
        }
    }
}
