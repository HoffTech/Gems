// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Linq;
using Gems.Metrics;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Exceptions;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Metrics;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Repository;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Settings;
using Gems.Utils;

using Microsoft.Extensions.Logging;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync;

public class ChangeTrackingSyncTableProcessor<TSourceEntity, TDestinationEntity, TMergeResult>
    : IChangeTrackingSyncTableProcessor<TSourceEntity, TDestinationEntity, TMergeResult>
    where TSourceEntity : class, ISourceChangeTrackingEntity
    where TDestinationEntity : class
    where TMergeResult : class, new()
{
    private const int IdenticalIterationsThreshold = 3;

    private readonly DestinationSyncedInfoProvider destinationSyncedInfoProvider;
    private readonly DestinationSyncedInfoUpdater destinationSyncedInfoUpdater;
    private readonly SourceDbChangeTrackingInfoProvider changeTrackingInfoProvider;
    private readonly SourceEntitiesProvider sourceEntitiesProvider;
    private readonly DestinationEntitiesUpdater destinationEntitiesUpdater;
    private readonly IMapper mapper;
    private readonly IMetricsService metricsService;
    private readonly ILogger<IChangeTrackingSyncTableProcessor<TSourceEntity, TDestinationEntity, TMergeResult>> logger;

    public ChangeTrackingSyncTableProcessor(
        DestinationSyncedInfoProvider destinationSyncedInfoProvider,
        DestinationSyncedInfoUpdater destinationSyncedInfoUpdater,
        SourceDbChangeTrackingInfoProvider changeTrackingInfoProvider,
        SourceEntitiesProvider sourceEntitiesProvider,
        DestinationEntitiesUpdater destinationEntitiesUpdater,
        IMapper mapper,
        IMetricsService metricsService,
        ILogger<IChangeTrackingSyncTableProcessor<TSourceEntity, TDestinationEntity, TMergeResult>> logger)
    {
        this.destinationSyncedInfoProvider = destinationSyncedInfoProvider;
        this.destinationSyncedInfoUpdater = destinationSyncedInfoUpdater;
        this.changeTrackingInfoProvider = changeTrackingInfoProvider;
        this.sourceEntitiesProvider = sourceEntitiesProvider;
        this.destinationEntitiesUpdater = destinationEntitiesUpdater;
        this.mapper = mapper;
        this.metricsService = metricsService;
        this.logger = logger;
    }

    public async Task<SyncTableResult<TMergeResult>> Sync(
        ChangeTrackingSyncInfo syncInfo,
        CancellationToken cancellationToken)
    {
        var sessionContext = new SessionContext<TMergeResult>();

        while (sessionContext.NeedToSyncDataAgain)
        {
            sessionContext.DestinationInfo = await this.destinationSyncedInfoProvider
                .GetLastSyncedInfoForTableAsync(
                    syncInfo.DestinationSettings.DbKey,
                    syncInfo.DestinationSettings.TableName,
                    cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(sessionContext.DestinationInfo?.TableName))
            {
                sessionContext.DestinationInfo = new SyncedInfo { TableName = syncInfo.DestinationSettings.TableName, Version = 0 };
            }

            /*
             * сохраняем версию перед чтением, что-бы не пропустить изменения которые могут произойти во время чтения
             * одни и теже изменения могут быть сохранены в целевую бд более одного раза (логигка AT LEAST ONCE)
             * процесс сохранения и обработки данных в целевой бд должен быть идемпотентным
             */
            sessionContext.SourceChangeTrackingVersionInfo = await this.changeTrackingInfoProvider
                .GetCurrentChangeTrackingInfoAsync(
                    syncInfo.SourceSettings.DbKey,
                    syncInfo.SourceSettings.TableName,
                    syncInfo.SourceSettings.PrimaryKeyName,
                    cancellationToken);

            if (!sessionContext.SourceChangeTrackingVersionInfo.CurrentVersion.HasValue
                || !sessionContext.SourceChangeTrackingVersionInfo.MinValidVersion.HasValue)
            {
                throw new SyncException(
                    $"Change tracking is not avalible in Source DB ({syncInfo.SourceSettings.DbKey})");
            }

            // чтобы понимать какая версия источника была на момент старта сессии синхронизации
            if (sessionContext.SessionSourceChangeTrackingVersionInfo == null)
            {
                sessionContext.SessionSourceChangeTrackingVersionInfo = sessionContext.SourceChangeTrackingVersionInfo;
                sessionContext.Offset = sessionContext.SessionSourceChangeTrackingVersionInfo.MinKey.GetValueOrDefault(0);
            }

            sessionContext.IsFullReloadSession ??= this.CheckDestinationNeedsToBeReload(
                syncInfo,
                sessionContext.SourceChangeTrackingVersionInfo,
                sessionContext.DestinationInfo);

            if (sessionContext.IsFullReloadSession.Value)
            {
                await this.FullReloadSyncSessionIteration(syncInfo, sessionContext, cancellationToken);
            }
            else
            {
                await this.ChangesSyncSessionIteration(syncInfo, sessionContext, cancellationToken);
            }

            var currentBatch = sessionContext.Results.Last();

            if (sessionContext.Results.Count >= IdenticalIterationsThreshold
                && sessionContext.Results.TakeLast(IdenticalIterationsThreshold)
                    .All(r =>
                        r.version == currentBatch.version
                        && r.offset == currentBatch.offset))
            {
                throw new SyncException(
                    $"Sync process {syncInfo.SourceSettings.DbKey} => {syncInfo.DestinationSettings.DbKey} halted, " +
                    $"last iterations completed with indentical parameters");
            }
        }

        return new SyncTableResult<TMergeResult>(
            sessionContext.IsFullReloadSession.GetValueOrDefault(false),
            sessionContext.Results
            .Select(br => br.mergeResult)
            .ToArray());
    }

    private async Task ChangesSyncSessionIteration(
        ChangeTrackingSyncInfo syncInfo,
        SessionContext<TMergeResult> sessionContext,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        var sourceData = await this.sourceEntitiesProvider
            .LoadChangesFromVersion<TSourceEntity>(
                syncInfo.SourceSettings.DbKey,
                sessionContext.DestinationInfo.Version,
                syncInfo.SourceSettings.ChangesQuery,
                syncInfo.SourceSettings.GetCommandTimeout,
                syncInfo.SourceSettings.BatchSize,
                cancellationToken)
            .ConfigureAwait(false);

        await this.metricsService.Histogram(
            ChangeTrackingSyncMetrics.ChangesSourceLoadTimeHistogram,
            sw.ElapsedMilliseconds,
            typeof(TDestinationEntity).Name);
        await this.metricsService.Histogram(
            ChangeTrackingSyncMetrics.ChangesDataCountHistogram,
            sourceData.Count,
            typeof(TDestinationEntity).Name);
        await this.metricsService.Histogram(
            ChangeTrackingSyncMetrics.ChangesTransactionCountHistogram,
            sourceData.GroupBy(x => x.ChangeTrackingVersion).Count(),
            typeof(TDestinationEntity).Name);

        var (total, inserts, updates, deletes) = this.CountSourceDataStatistics(sourceData);

        this.logger.LogTrace(
            "ChangeTrackingSync process {0} => {1} changes data load, " +
            "version: {2} loaded: {3} (i:{4} u:{5} d:{6}) time: {7}",
            syncInfo.SourceSettings.DbKey,
            syncInfo.DestinationSettings.DbKey,
            sessionContext.DestinationInfo.Version,
            total,
            inserts,
            updates,
            deletes,
            sw.ElapsedMilliseconds);

        await this.metricsService.Counter(ChangeTrackingSyncMetrics.ChangesInsertCountCounter, inserts);
        await this.metricsService.Counter(ChangeTrackingSyncMetrics.ChangesUpdateCountCounter, updates);
        await this.metricsService.Counter(ChangeTrackingSyncMetrics.ChangesDeleteCountCounter, deletes);

        // определяем нужно ли продолжать вычитывать следующий batch
        sessionContext.NeedToSyncDataAgain = this.CheckIsNeedToSyncChangesDataAgain(
            sessionContext.SessionSourceChangeTrackingVersionInfo.CurrentVersion,
            sessionContext.DestinationInfo,
            sourceData,
            syncInfo.SourceSettings);

        // преобразование типов, обработка даты, и т.п.
        sw.Restart();
        var destinationEntities = await this.TransformAsync(syncInfo, sourceData);
        this.logger.LogTrace("transform time: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        await this.metricsService.Histogram(
            ChangeTrackingSyncMetrics.BatchTransformTimeHistogram,
            sw.ElapsedMilliseconds,
            typeof(TDestinationEntity).Name);

        // фиксируем версию для следующего чтения
        // используем минимальную из последней версии порции и версией полученной из источника перед загрузкой изменений
        sessionContext.DestinationInfo.Version = Math.Min(
            sessionContext.SourceChangeTrackingVersionInfo.CurrentVersion!.Value,
            (sourceData.LastOrDefault()?.ChangeTrackingVersion).GetValueOrDefault(long.MaxValue));
        sessionContext.DestinationInfo.LastRestoreDateTime = sessionContext.SourceChangeTrackingVersionInfo.LastRestoreDateTime;
        sessionContext.DestinationInfo.UpdateTime = DateTime.UtcNow;

        sw.Restart();

        var mergeResult = await this.MergeEntitiesAsync(
            destinationEntities,
            sessionContext.DestinationInfo,
            syncInfo.DestinationSettings,
            needCleanup: false,
            cancellationToken).ConfigureAwait(false);

        if (mergeResult is IDestinationEntitiesHolder<TDestinationEntity> destinationEntitiesHolder)
        {
            destinationEntitiesHolder.Entities = destinationEntities;
        }

        this.logger.LogTrace(
            "ChangeTrackingSync process {0} => {1} data saved, time: {2} ms",
            syncInfo.SourceSettings.DbKey,
            syncInfo.DestinationSettings.DbKey,
            sw.ElapsedMilliseconds);
        await this.metricsService.Histogram(
            ChangeTrackingSyncMetrics.ChangesSaveTimeHistogram,
            sw.ElapsedMilliseconds,
            typeof(TDestinationEntity).Name);

        var currentBatch = (sessionContext.DestinationInfo.Version, 0, mergeResult);
        sessionContext.Results.Add(currentBatch);
    }

    private async Task FullReloadSyncSessionIteration(
        ChangeTrackingSyncInfo syncInfo,
        SessionContext<TMergeResult> sessionContext,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        var sourceData = await this.sourceEntitiesProvider
            .LoadFull<TSourceEntity>(
                syncInfo.SourceSettings.DbKey,
                syncInfo.SourceSettings.FullReloadQuery,
                syncInfo.SourceSettings.GetCommandTimeout,
                sessionContext.Offset,
                syncInfo.SourceSettings.BatchSize,
                cancellationToken)
            .ConfigureAwait(false);

        this.logger.LogTrace(
            "ChangeTrackingSync process {0} => {1} full data load, " +
            "offset: {2} loaded: {3} time: {4}",
            syncInfo.SourceSettings.DbKey,
            syncInfo.DestinationSettings.DbKey,
            sessionContext.Offset,
            sourceData.Count,
            sw.ElapsedMilliseconds);

        await this.metricsService.Histogram(
            ChangeTrackingSyncMetrics.FullLoadSourceLoadTimeHistogram,
            sw.ElapsedMilliseconds,
            typeof(TDestinationEntity).Name);
        await this.metricsService.Histogram(
            ChangeTrackingSyncMetrics.FullLoadDataCountHistogram,
            sourceData.Count,
            typeof(TDestinationEntity).Name);
        await this.metricsService.Histogram(
            ChangeTrackingSyncMetrics.FullLoadTransactionCountHistogram,
            sourceData.GroupBy(x => x.ChangeTrackingVersion).Count(),
            typeof(TDestinationEntity).Name);

        await this.metricsService.Counter(ChangeTrackingSyncMetrics.FullloadInsertCountCounter, sourceData.Count);

        sessionContext.Offset += syncInfo.SourceSettings.BatchSize;

        // определяем нужно ли продолжать вычитывать следующий batch
        sessionContext.NeedToSyncDataAgain =
            sessionContext.Offset < sessionContext.SourceChangeTrackingVersionInfo.MaxKey;

        // преобразование типов, обработка даты, и т.п.
        sw.Restart();
        var destinationEntities = await this.TransformAsync(syncInfo, sourceData);
        this.logger.LogTrace("transform time: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
        await this.metricsService.Histogram(
            ChangeTrackingSyncMetrics.BatchTransformTimeHistogram,
            sw.ElapsedMilliseconds,
            typeof(TDestinationEntity).Name);

        // фиксируем версию для следующего чтения
        // используем минимальную из последней версии порции и версией полученной из источника перед загрузкой изменений
        sessionContext.DestinationInfo.Version =
            sessionContext.NeedToSyncDataAgain ? 0L : sessionContext.SessionSourceChangeTrackingVersionInfo.CurrentVersion!.Value;
        sessionContext.DestinationInfo.LastRestoreDateTime = sessionContext.SourceChangeTrackingVersionInfo.LastRestoreDateTime;
        sessionContext.DestinationInfo.UpdateTime = DateTime.UtcNow;

        sw.Restart();
        var needToClearData = !sessionContext.SessionDataCleared;

        var mergeResult = await this.MergeEntitiesAsync(
            destinationEntities,
            sessionContext.DestinationInfo,
            syncInfo.DestinationSettings,
            needToClearData,
            cancellationToken).ConfigureAwait(false);
        this.logger.LogTrace(
            "ChangeTrackingSync process {0} => {1} data saved, time: {2} ms",
            syncInfo.SourceSettings.DbKey,
            syncInfo.DestinationSettings.DbKey,
            sw.ElapsedMilliseconds);
        await this.metricsService.Histogram(
            ChangeTrackingSyncMetrics.FullLoadSaveTimeHistogram,
            sw.ElapsedMilliseconds,
            typeof(TDestinationEntity).Name);

        if (needToClearData)
        {
            sessionContext.SessionDataCleared = true;
        }

        var currentBatch = (sessionContext.DestinationInfo.Version, sessionContext.Offset, mergeResult);
        sessionContext.Results.Add(currentBatch);
    }

    private (int total, int inserts, int updates, int deletes) CountSourceDataStatistics(
        List<TSourceEntity> sourceData)
    {
        var (total, inserts, updates, deletes) = (0, 0, 0, 0);
        foreach (var data in sourceData)
        {
            total++;
            switch (data.OperationType)
            {
                case "I":
                    inserts++;
                    break;
                case "U":
                    updates++;
                    break;
                case "D":
                    deletes++;
                    break;
                default:
                    throw new NotImplementedException($"Operation type: {data.OperationType}");
            }
        }

        return (total, inserts, updates, deletes);
    }

    private bool CheckIsNeedToSyncChangesDataAgain(
        long? sessionSourceVersion,
        SyncedInfo destinationInfo,
        IReadOnlyCollection<TSourceEntity> sourceData,
        SourceDataSettings sourceSettings)
    {
        // завершаем сессию синхронизации, если долши до версии которая была в начале сессии
        if (sessionSourceVersion < destinationInfo.Version)
        {
            return false;
        }

        // текущая порция пустая, можно завершать сессию
        if (sourceData.IsNullOrEmpty())
        {
            return false;
        }

        // текущая порция мешьшего объема чем максимальный размер
        if (sourceData.Count < sourceSettings.BatchSize)
        {
            return false;
        }

        return true;
    }

    private async Task<TMergeResult> MergeEntitiesAsync(
        List<TDestinationEntity> entities,
        SyncedInfo destinationInfo,
        DestinationSettings destinationSettings,
        bool needCleanup,
        CancellationToken cancellationToken)
    {
        if (needCleanup)
        {
            // при полной перезаливке данных должны очистить, независимо от того есть ли хоть какие-то данные в источнике сейчас
            await this.destinationEntitiesUpdater
                .ClearDestinationAsync(
                    destinationSettings.DbKey,
                    destinationSettings.ClearFunctionName,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        var mergeResult = new TMergeResult();
        if (!entities.IsNullOrEmpty())
        {
            mergeResult = await this.destinationEntitiesUpdater
                .MergeEntitiesAsync<TDestinationEntity, TMergeResult>(
                    destinationSettings.DbKey,
                    entities,
                    destinationSettings.EnableFullChangesLog,
                    destinationSettings.MergeFunctionName,
                    destinationSettings.MergeParameterName,
                    destinationSettings.MergeCommandTimeout,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        // что-бы отличать ситуацию когда синхронизация давно не отрабатывала, от ситуации когда исходные данные давно не менялись
        // например в случае если исходная таблица меняется редко и CHANGE_RETENTION уже очистил данные
        // необходимо сохранять destination info всегда, даже еслм данных нету
        await this.destinationSyncedInfoUpdater
            .UpsertChangeTrackingInfoForTableAsync(
                destinationSettings.DbKey,
                destinationSettings.TableName,
                destinationInfo,
                cancellationToken)
            .ConfigureAwait(false);

        return mergeResult;
    }

    private Task<List<TDestinationEntity>> TransformAsync(
        ChangeTrackingSyncInfo syncInfo,
        List<TSourceEntity> sourceData)
    {
        if (syncInfo.NeedConvertDateTimeToUtc)
        {
            DateTimeUtils.SetUnspecifiedToUtcDateTimeEnumerable(sourceData);
        }

        // TODO Add customizable transformation logic
        var entities = this.mapper.Map<List<TDestinationEntity>>(sourceData);

        return Task.FromResult(entities);
    }

    private bool CheckDestinationNeedsToBeReload(
        ChangeTrackingSyncInfo syncInfo,
        ChangeTrackingInfo sourceChangeTrackingVersionInfo,
        SyncedInfo currentDestinationSyncedInfo)
    {
        // в бд источнике было восстановлени бд с момента последнего синка
        if (currentDestinationSyncedInfo.LastRestoreDateTime.GetValueOrDefault(DateTime.MinValue) <
            sourceChangeTrackingVersionInfo.LastRestoreDateTime)
        {
            var needFullReload = syncInfo.SourceSettings.OnRestoreFromBackupDetected switch
            {
                // падаем и ждем вмешательства извне
                SyncErrorAction.Fail => throw new SourceDbRestoredFromBackupException(
                    this.BuildSourceDbErrorRestoredMessage(
                        sourceChangeTrackingVersionInfo,
                        currentDestinationSyncedInfo,
                        syncInfo)),

                // просто уведомляем о проблеме
                SyncErrorAction.Log => this.LogOnly(
                    this.BuildSourceDbErrorRestoredMessage(
                        sourceChangeTrackingVersionInfo,
                        currentDestinationSyncedInfo,
                        syncInfo)),

                // восстанавливаем данные автоматически
                SyncErrorAction.FullReload => true,
                _ => throw new NotImplementedException()
            };

            if (needFullReload)
            {
                return true;
            }
        }

        if (sourceChangeTrackingVersionInfo.MinValidVersion <= currentDestinationSyncedInfo.Version)
        {
            // все ок, в источнике есть вся необходимая история
            return false;
        }

        // в бд назначения устаревшие данные,
        // в бд источнике уже нет изменений необходимых для синхронизации данных
        return syncInfo.SourceSettings.OnDestinationVersionOutdated switch
        {
            // падаем и ждем вмешательства извне
            SyncErrorAction.Fail => throw new OutdatedDestinationDataException(
                this.BuildDestinationDbOutdatedErrorMessage(
                    sourceChangeTrackingVersionInfo,
                    currentDestinationSyncedInfo,
                    syncInfo)),

            // просто уведомляем о проблеме
            SyncErrorAction.Log => this.LogOnly(
                this.BuildDestinationDbOutdatedErrorMessage(
                    sourceChangeTrackingVersionInfo,
                    currentDestinationSyncedInfo,
                    syncInfo)),

            // восстанавливаем данные автоматически
            SyncErrorAction.FullReload => true,
            _ => throw new NotImplementedException()
        };

        // TODO destination run forward
        // if (sourceInfo.CurrentVersion < destinationInfo.Version)
        // {
        //     return false;
        // }
    }

    private string BuildDestinationDbOutdatedErrorMessage(
        ChangeTrackingInfo sourceChangeTrackingVersionInfo,
        SyncedInfo currentDestinationSyncedInfo,
        ChangeTrackingSyncInfo syncInfo)
    {
        return $"Sync from: {syncInfo.SourceSettings.DbKey}.{syncInfo.SourceSettings.TableName} failed " +
               $"source hvae min valid version: {sourceChangeTrackingVersionInfo.MinValidVersion} " +
               $"destination need changes from version: {currentDestinationSyncedInfo.Version}";
    }

    private string BuildSourceDbErrorRestoredMessage(
        ChangeTrackingInfo sourceChangeTrackingVersionInfo,
        SyncedInfo currentDestinationSyncedInfo,
        ChangeTrackingSyncInfo syncInfo)
    {
        return $"Sync from: {syncInfo.SourceSettings.DbKey}.{syncInfo.SourceSettings.TableName} failed " +
               $"source db restored from backup at: {sourceChangeTrackingVersionInfo.LastRestoreDateTime} " +
               $"last synced backup moment: {currentDestinationSyncedInfo.LastRestoreDateTime}";
    }

    private bool LogOnly(string errorText)
    {
        this.logger.LogError(errorText);
        return false;
    }
}
