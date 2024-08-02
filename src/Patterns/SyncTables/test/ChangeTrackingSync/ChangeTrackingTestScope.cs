// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;
using Gems.Metrics;
using Gems.Patterns.SyncTables.ChangeTrackingSync;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Settings;
using Gems.Patterns.SyncTables.Options;
using Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync;

public class ChangeTrackingTestScope : IAsyncDisposable
{
    private readonly Mock<IUnitOfWork> unitOfWorkMock;

    private ChangeTrackingTestScope(
        Mock<IUnitOfWork> unitOfWorkMock,
        ServiceProvider serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
        this.unitOfWorkMock = unitOfWorkMock;
    }

    public ServiceProvider ServiceProvider { get; private set; }

    public static ChangeTrackingTestScope Build()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddAutoMapper(cfg => { cfg.AddProfile<ClientsMappingProfile>(); });

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ChangeTrackingSyncOptions:UpsertVersionFunctionInfo:FunctionName"] = "UpsertVersionFunction",
                ["ChangeTrackingSyncOptions:UpsertVersionFunctionInfo:TableParameterName"] = "TableParameterName",
                ["ChangeTrackingSyncOptions:UpsertVersionFunctionInfo:RowVersionParameterName"] =
                    "RowVersionParameterName",
                ["ChangeTrackingSyncOptions:ProviderVersionFunctionInfo:FunctionName"] = "ProviderVersionFunction",
                ["ChangeTrackingSyncOptions:ProviderVersionFunctionInfo:TableParameterName"] = "TableParameterName"
            })
            .Build();

        services.AddChangeTrackingTableSyncer(configuration.GetSection(nameof(ChangeTrackingSyncOptions)));

        services.AddSingleton(Mock.Of<IMetricsService>());
        services.AddSingleton(Mock.Of<IUnitOfWorkProvider>());

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var unitOfWorkProviderMock = new Mock<IUnitOfWorkProvider>();
        unitOfWorkProviderMock
            .Setup(x =>
                x.GetUnitOfWork(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(() => unitOfWorkMock.Object);

        services.AddSingleton(unitOfWorkProviderMock.Object);

        var serviceProvider = services.BuildServiceProvider();

        var scope = new ChangeTrackingTestScope(unitOfWorkMock, serviceProvider);
        return scope;
    }

    public void CheckVersionForNextSync(long? newVersion, DateTime? lastRestoreDateTime)
    {
        var syncOptions = this.ServiceProvider.GetRequiredService<IOptions<ChangeTrackingSyncOptions>>().Value;
        if (newVersion.HasValue)
        {
            this.unitOfWorkMock.Verify(
                x => x.CallStoredProcedureAsync(
                    It.Is<string>(c => c == syncOptions.UpsertVersionFunctionInfo.FunctionName),
                    It.Is<Dictionary<string, object>>(
                        d => ((SyncedInfo)d[syncOptions.UpsertVersionFunctionInfo.RowVersionParameterName]).Version ==
                             newVersion
                             && ((SyncedInfo)d[syncOptions.UpsertVersionFunctionInfo.RowVersionParameterName])
                             .LastRestoreDateTime == lastRestoreDateTime),
                    null),
                Times.AtLeast(1));
        }
    }

    public void CheckDestinationSave(
        TestExpectations expectations,
        int? testDataCount,
        DestinationSettings destinationSettings)
    {
        if (testDataCount is null or <= 0)
        {
            return;
        }

        var expectedEntitiesCount =
            Math.Min(testDataCount.Value, expectations.BatchSize.GetValueOrDefault(int.MaxValue));

        if (expectations.SaveIterations > 1)
        {
            this.unitOfWorkMock.Verify(
                x => x.CallTableFunctionFirstAsync<TestSyncResult>(
                    It.Is<string>(c => c == destinationSettings.MergeFunctionName),
                    It.IsAny<int>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null),
                Times.Exactly(expectations.SaveIterations.Value));

            var saves = this.unitOfWorkMock
                .Invocations.Where(i =>
                    i.Arguments.Count > 0
                    && i.Arguments[0] != null
                    && i.Arguments[0].ToString() == destinationSettings.MergeFunctionName)
                .ToArray();

            var total = saves.Sum(invocation =>
                invocation.Arguments
                    .OfType<Dictionary<string, object>>()
                    .Select(d => ((List<RealDestinationEntity>)d[destinationSettings.MergeParameterName]).Count)
                    .First());

            Assert.That(total, Is.EqualTo(testDataCount));
        }
        else
        {
            this.unitOfWorkMock.Verify(
                x => x.CallTableFunctionFirstAsync<TestSyncResult>(
                    It.Is<string>(c => c == destinationSettings.MergeFunctionName),
                    It.IsAny<int>(),
                    It.Is<Dictionary<string, object>>(
                        d => ((List<RealDestinationEntity>)d[destinationSettings.MergeParameterName]).Count ==
                             expectedEntitiesCount),
                    null),
                Times.Once);
        }

        if (expectations.IsFullLoad)
        {
            this.unitOfWorkMock.Verify(
                x => x.CallStoredProcedureAsync(
                    It.Is<string>(c => c == destinationSettings.ClearFunctionName),
                    null),
                Times.Exactly(1));
        }
    }

    public void ChekSourceLoading(TestExpectations expectations, long? targetTableVersion)
    {
        if (expectations.IsFullLoad)
        {
            this.unitOfWorkMock.Verify(
                x => x.QueryAsync<RealSourceChangeTrackingEntity>(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.Is<Dictionary<string, object>>(
                        d => !expectations.BatchSize.HasValue || (int)d["@batchSize"] == expectations.BatchSize.Value),
                    null),
                Times.Exactly(expectations.LoadIterations.GetValueOrDefault(1)));
        }
        else
        {
            this.unitOfWorkMock.Verify(
                x => x.QueryAsync<RealSourceChangeTrackingEntity>(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.Is<Dictionary<string, object>>(d =>
                        (long)d["@version"] == targetTableVersion
                        && (!expectations.BatchSize.HasValue || (int)d["@batchSize"] == expectations.BatchSize.Value)),
                    null),
                Times.Once);

            this.unitOfWorkMock.Verify(
                x => x.QueryAsync<RealSourceChangeTrackingEntity>(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.Is<Dictionary<string, object>>(d =>
                        !expectations.BatchSize.HasValue || (int)d["@batchSize"] == expectations.BatchSize.Value),
                    null),
                Times.Exactly(expectations.LoadIterations.GetValueOrDefault(1)));
        }
    }

    public void SetupSources(
        long? sourceDbVersion,
        long? sourceDbMinVersion,
        DateTime? lastRestoreDateTime,
        int? testDataCount,
        ChangeTrackingSyncInfo syncInfo)
    {
        this.unitOfWorkMock
            .Setup(
                u => u.QueryFirstOrDefaultAsync<ChangeTrackingInfo>(
                    It.IsAny<string>(),
                    It.IsAny<Enum>()))
            .ReturnsAsync(
                new ChangeTrackingInfo
                {
                    CurrentVersion = sourceDbVersion,
                    MinValidVersion = sourceDbMinVersion,
                    LastRestoreDateTime = lastRestoreDateTime,
                    MaxKey = testDataCount,
                    MinKey = 0
                });

        this.unitOfWorkMock
            .Setup(
                u => u.QueryAsync<RealSourceChangeTrackingEntity>(
                    It.Is<string>(q => q == syncInfo.SourceSettings.FullReloadQuery),
                    It.IsAny<int>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null))
            .ReturnsAsync((string _, int _, Dictionary<string, object> p, Enum _) =>
            {
                var batchSize = (int)p["@batchSize"];
                var offset = (long)p["@offset"];
                var fromVersion = (int)(sourceDbVersion!.Value - testDataCount.GetValueOrDefault(0) + 1);
                return Enumerable
                    .Range(fromVersion, testDataCount.GetValueOrDefault(0))
                    .Skip((int)offset)
                    .Take(batchSize)
                    .Select(x =>
                        new RealSourceChangeTrackingEntity { OperationType = "U", ChangeTrackingVersion = x })
                    .ToList();
            });

        this.unitOfWorkMock
            .Setup(
                u => u.QueryAsync<RealSourceChangeTrackingEntity>(
                    It.Is<string>(q => q == syncInfo.SourceSettings.ChangesQuery),
                    It.IsAny<int>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<Enum>()))
            .ReturnsAsync((string _, int _, Dictionary<string, object> p, Enum _) =>
            {
                var from = (long)p["@version"];
                var batchSize = (int)p["@batchSize"];

                var changesCount = (int)Math.Max(0, sourceDbVersion.GetValueOrDefault(0) - from);
                return Enumerable
                    .Range((int)from + 1, changesCount)
                    .Take(batchSize)
                    .Select(x =>
                        new RealSourceChangeTrackingEntity { OperationType = "U", ChangeTrackingVersion = x })
                    .ToList();
            });

        this.unitOfWorkMock
            .Setup(
                x => x.CallTableFunctionFirstAsync<TestSyncResult>(
                    It.Is<string>(c => c == syncInfo.DestinationSettings.MergeFunctionName),
                    It.IsAny<Dictionary<string, object>>(),
                    null))
            .ReturnsAsync(new TestSyncResult());
    }

    public void SetupDestination(long? destinationTableVersion, DateTime? restoreDateTimeSyncedToDestination)
    {
        if (!destinationTableVersion.HasValue)
        {
            return;
        }

        this.unitOfWorkMock
            .Setup(u => u.CallScalarFunctionAsync<SyncedInfo>(
                It.Is<string>(fn => fn == "ProviderVersionFunction"),
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<Enum>()))
            .ReturnsAsync(
                new SyncedInfo
                {
                    LastRestoreDateTime = restoreDateTimeSyncedToDestination,
                    Version = destinationTableVersion.Value,
                    TableName = "sync_table",
                    UpdateTime = DateTime.UtcNow
                });
    }

    public ValueTask DisposeAsync()
    {
        return this.ServiceProvider is IAsyncDisposable disposable
            ? disposable.DisposeAsync()
            : ValueTask.CompletedTask;
    }
}
