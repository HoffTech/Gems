// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Patterns.SyncTables.ChangeTrackingSync;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Exceptions;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Settings;
using Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.Entities;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync;

public class ChangeTrackingSyncProcessTests
{
    [Test]

    // CT not enabled
    [TestCase(null, null, null, null, false, typeof(SyncException))]

    // Read from target version
    [TestCase(0, 0, 0, null, false, null)]
    [TestCase(0, 0, 10000, null, false, null)]

    // Sync just added, need to full sync, because of min version
    [TestCase(10000, 1000, null, null, true, null)]

    // Check min version, reload all if target to late
    [TestCase(10000, 1000, 500, 10000, true, null)]

    // Update target even if no records
    [TestCase(1000, 0, 500, 1000, false, null)]
    public async Task Sync_By_Change_Tracking_Tests(
        long? sourceDbVersion,
        long? sourceDbMinVersion,
        long? destinationTableVersion,
        long? expectNewVersion,
        bool expectFullLoad,
        Type expectedExceptionType)
    {
        var buildMergeInfo = () => new ChangeTrackingSyncInfo(
            new SourceDataSettings
            {
                DbKey = "axapta",
                TableName = "source",
                ChangesQuery = "source_query",
                FullReloadQuery = "source_full_query",
                OnDestinationVersionOutdated = SyncErrorAction.FullReload
            },
            new DestinationSettings
            {
                DbKey = "default",
                TableName = "sync_table",
                MergeFunctionName = "public.merge_function",
                MergeParameterName = "p_entities",
                ClearFunctionName = "clear_data"
            },
            needConvertDateTimeToUtc: false);

        await RunSyncTest(
            sourceDbVersion,
            sourceDbMinVersion,
            destinationTableVersion,
            sourceRestoredDateTime: null,
            restoreDateTimeSyncedToDestination: null,
            new TestExpectations
            {
                NewVersion = expectNewVersion,
                IsFullLoad = expectFullLoad,
                ExceptionType = expectedExceptionType,
            },
            buildMergeInfo);
    }

    [Test]

    // throw exception in Fail mode
    [TestCase(SyncErrorAction.Fail, false, typeof(OutdatedDestinationDataException))]

    // do compltete sync in reload in FullReload mode
    [TestCase(SyncErrorAction.FullReload, true, null)]

    // just log warning and leave destination database unconsistent
    [TestCase(SyncErrorAction.Log, false, null)]
    public async Task Min_Change_Tracking_Version_Check_Test(
        SyncErrorAction outdatedAction,
        bool expectFullLoad,
        Type expectedExceptionType)
    {
        var buildMergeInfo = () => new ChangeTrackingSyncInfo(
            new SourceDataSettings
            {
                DbKey = "axapta",
                TableName = "source",
                ChangesQuery = "source_query",
                FullReloadQuery = "source_full_query",
                OnDestinationVersionOutdated = outdatedAction
            },
            new DestinationSettings
            {
                DbKey = "default",
                TableName = "sync_table",
                MergeFunctionName = "public.merge_function",
                MergeParameterName = "p_entities",
                ClearFunctionName = "clear_data"
            },
            needConvertDateTimeToUtc: false);

        await RunSyncTest(
            sourceDbVersion: 10000,
            sourceDbMinVersion: 1000,
            destinationTableVersion: 500,
            sourceRestoredDateTime: null,
            restoreDateTimeSyncedToDestination: null,
            new TestExpectations
            {
                NewVersion = 10000,
                IsFullLoad = expectFullLoad,
                ExceptionType = expectedExceptionType,
            },
            buildMergeInfo);
    }

    // throw exception in Fail mode, it will stop sync process and problem MUST be escalated to support
    [TestCase(SyncErrorAction.Fail, "2023.10.27", "2023.10.20", "source_full_query", false, typeof(SourceDbRestoredFromBackupException))]

    // just log warning and leave destination database unconsistent
    [TestCase(SyncErrorAction.Log, "2023.10.27", "2023.10.20", "source_full_query", false, null)]

    // no need sync
    [TestCase(SyncErrorAction.FullReload, "", "", "source_full_query", false, null)]
    [TestCase(SyncErrorAction.FullReload, "2023.10.20", "2023.10.27", "source_full_query", false, null)]

    // do complete sync
    [TestCase(SyncErrorAction.FullReload, "2023.10.27", "2023.10.20", "source_full_query", true, null)]
    [TestCase(SyncErrorAction.FullReload, "2023.10.27", "", "source_full_query", true, null)]

    // bad configuration
    [TestCase(SyncErrorAction.FullReload, "2023.10.27", "2023.10.20", null, true, typeof(SyncBadConfigurationException))]
    public async Task Source_DB_Restored_From_Backup_Check_Test(
        SyncErrorAction outdatedAction,
        string sourceRestoredDateTime,
        string restoreDateTimeSyncedToDestination,
        string fullReloadQuery,
        bool expectFullLoad,
        Type expectedExceptionType)
    {
        var buildMergeInfo = () => new ChangeTrackingSyncInfo(
                new SourceDataSettings
                {
                    DbKey = "axapta",
                    TableName = "source",
                    ChangesQuery = "source_query",
                    FullReloadQuery = fullReloadQuery,
                    OnDestinationVersionOutdated = SyncErrorAction.Log,
                    OnRestoreFromBackupDetected = outdatedAction
                },
                new DestinationSettings
                {
                    DbKey = "default",
                    TableName = "sync_table",
                    MergeFunctionName = "public.merge_function",
                    MergeParameterName = "p_entities",
                    ClearFunctionName = "clear_data"
                },
                needConvertDateTimeToUtc: false);

        await RunSyncTest(
            sourceDbVersion: 10000,
            sourceDbMinVersion: 0,
            destinationTableVersion: 500,
            sourceRestoredDateTime: string.IsNullOrEmpty(sourceRestoredDateTime) ? null : DateTime.Parse(sourceRestoredDateTime),
            restoreDateTimeSyncedToDestination: string.IsNullOrEmpty(restoreDateTimeSyncedToDestination) ? null : DateTime.Parse(restoreDateTimeSyncedToDestination),
            new TestExpectations
            {
                NewVersion = 10000,
                IsFullLoad = expectFullLoad,
                ExceptionType = expectedExceptionType,
            },
            buildMergeInfo);
    }

    // Batched sync tests
    [Test]

    // changes load from version with multiple batch

    // all batch contain same version => should load all this version changes in one batch

    // warning: slippage when batch size comparable with one transaction changes count

    // loaded full batch, need to try load next changes
    [TestCase(100, 0, 0, 100, false, 100, 2, 1, null)]

    // load rows less than default batch
    [TestCase(100, 0, 0, 100, false, null, 1, 1, null)]

    // load rows less than batch
    [TestCase(100, 0, 0, 100, false, 5000, 1, 1, null)]

    // multiple load
    [TestCase(500, 0, 0, 500, false, 100, 6, 5, null)]

    // multiple load
    [TestCase(501, 0, 0, 501, false, 100, 6, 6, null)]

    // full load multiple batch
    [TestCase(100, 1, 0, 100, true, 5000, 1, 1, null)]

    // full load multiple batch
    [TestCase(10000, 1, 0, 10000, true, 5000, 2, 2, null)]

    // full load multiple batch last batch not maximal size
    [TestCase(10001, 1, 0, 10001, true, 5000, 3, 3, null)]

    // full load multiple batch
    [TestCase(10000, 1000, 0, 10000, true, 500, 20, 20, null)]
    public async Task Sync_With_Batching_Tests(
        long? sourceDbVersion,
        long? sourceDbMinVersion,
        long? destinationTableVersion,
        long? expectNewVersion,
        bool expectFullLoad,
        int? expectedBatchSize,
        int? expectedLoadIterations,
        int? expectedSaveIterations,
        Type expectedExceptionType)
    {
        var buildMergeInfo = () => new ChangeTrackingSyncInfo(
            new SourceDataSettings
            {
                DbKey = "axapta",
                TableName = "source",
                ChangesQuery = "source_query",
                FullReloadQuery = "source_full_query",
                OnDestinationVersionOutdated = SyncErrorAction.FullReload,
                BatchSize = expectedBatchSize.GetValueOrDefault(100_000)
            },
            new DestinationSettings
            {
                DbKey = "default",
                TableName = "sync_table",
                MergeFunctionName = "public.merge_function",
                MergeParameterName = "p_entities",
                ClearFunctionName = "clear_data"
            },
            needConvertDateTimeToUtc: false);

        await RunSyncTest(
            sourceDbVersion,
            sourceDbMinVersion,
            destinationTableVersion,
            sourceRestoredDateTime: null,
            restoreDateTimeSyncedToDestination: null,
            new TestExpectations
            {
                NewVersion = expectNewVersion,
                IsFullLoad = expectFullLoad,
                ExceptionType = expectedExceptionType,
                BatchSize = expectedBatchSize,
                LoadIterations = expectedLoadIterations,
                SaveIterations = expectedSaveIterations
            },
            buildMergeInfo);
    }

    private static async Task RunSyncTest(
        long? sourceDbVersion,
        long? sourceDbMinVersion,
        long? destinationTableVersion,
        DateTime? sourceRestoredDateTime,
        DateTime? restoreDateTimeSyncedToDestination,
        TestExpectations expectations,
        Func<ChangeTrackingSyncInfo> buildSyncInfo)
    {
        await using var scope = ChangeTrackingTestScope.Build();

        Exception catchedException = null;
        try
        {
            var syncInfo = buildSyncInfo();

            var testDataCount = CalculateTestDataCount(sourceDbVersion, destinationTableVersion);
            scope.SetupSources(sourceDbVersion, sourceDbMinVersion, sourceRestoredDateTime, testDataCount, syncInfo);
            scope.SetupDestination(destinationTableVersion, restoreDateTimeSyncedToDestination);

            var processor =
                scope.ServiceProvider.GetRequiredService<IChangeTrackingSyncTableProcessor<RealSourceChangeTrackingEntity, RealDestinationEntity, TestSyncResult>>();

            var syncResult = await processor.Sync(syncInfo, CancellationToken.None);

            Assert.That(syncResult, Is.Not.Null);
            scope.ChekSourceLoading(expectations, destinationTableVersion);
            scope.CheckVersionForNextSync(expectations.NewVersion, sourceRestoredDateTime);
            scope.CheckDestinationSave(expectations, testDataCount, syncInfo.DestinationSettings);
        }
        catch (Exception e)
        {
            catchedException = e;
            if (expectations.ExceptionType != e.GetType())
            {
                throw;
            }
        }

        Assert.That(catchedException?.GetType(), Is.EqualTo(expectations.ExceptionType));
    }

    private static int? CalculateTestDataCount(long? sourceDbVersion, long? destinationTableVersion)
    {
        if (sourceDbVersion is null
            || destinationTableVersion is null)
        {
            return null;
        }

        var testDataCount = (int)(sourceDbVersion.Value - destinationTableVersion.Value);

        return testDataCount < 0 ? null : testDataCount;
    }
}
