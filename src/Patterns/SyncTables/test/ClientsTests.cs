// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Patterns.SyncTables.MergeProcessor.MergeInfos;
using Gems.Patterns.SyncTables.Tests.Infrastructure;
using Gems.Patterns.SyncTables.Tests.Infrastructure.Clients;

using NUnit.Framework;

namespace Gems.Patterns.SyncTables.Tests
{
    [Parallelizable]
    public partial class ClientsTests
    {
        [Test]
        public async Task ProcessCollectionAsync_UseChangeTrackingMergeProcessors_ShouldReturnValidMergeResult()
        {
            // Arrange
            const int mergeInfosCount = 2;

            await using var scope = new TestScope
            {
                SourceDbKey = "axapta",
                TargetDbKey = "default",
                ExternalChangeTrackingEntities = new List<RealExternalChangeTrackingEntity>
                {
                    new () { Name = "Name1", Age = "25" },
                    new () { Name = "Name2", Age = "30" },
                    new () { Name = "Name3", Age = "35" },
                },
                MergeResult = new MergeResult { Value = "Success" }
            };
            scope.BuildServiceProvider();

            var client = scope.GetRequiredService<ChangeTrackingMergeClient>();

            // Act
            var result = await client.ProcessMergesAsync(
                    new ChangeTrackingMergeInfo<MergeResult>(
                        sourceDbKey: "axapta",
                        targetDbKey: "default",
                        tableName: "sync_table_1",
                        externalSyncQuery: "external_query_1",
                        mergeFunctionName: "public.merge_function_1",
                        mergeParameterName: "p_entities",
                        needConvertDateTimeToUtc: true),
                    new ChangeTrackingMergeInfo<MergeResult>(
                        sourceDbKey: "axapta",
                        targetDbKey: "default",
                        tableName: "sync_table_2",
                        externalSyncQuery: "external_query_2",
                        mergeFunctionName: "public.merge_function_2",
                        mergeParameterName: "p_entities",
                        needConvertDateTimeToUtc: false),
                    CancellationToken.None)
                .ConfigureAwait(false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(mergeInfosCount));
            Assert.That(result.Select(r => r.Value), Is.All.EqualTo(scope.MergeResult.Value));

            scope.VerifyUpsertTableVersionCallTimesExactly(mergeInfosCount);
        }

        [Test]
        public async Task ProcessCollectionAsync_NonChangeTrackingMergeProcessors_ShouldReturnValidMergeResult()
        {
            // Arrange
            const int mergeInfosCount = 3;

            await using var scope = new TestScope
            {
                SourceDbKey = "axapta",
                TargetDbKey = "default",
                ExternalEntities = new List<RealExternalEntity>
                {
                    new () { Name = "Name1", Age = "25" },
                    new () { Name = "Name2", Age = "30" },
                    new () { Name = "Name3", Age = "35" },
                },
                MergeResult = new MergeResult { Value = "Success" }
            };
            scope.BuildServiceProvider();

            var client = scope.GetRequiredService<MergeClient>();

            // Act
            var result = await client.ProcessMergesAsync(
                    new MergeInfo<MergeResult>(
                        sourceDbKey: "axapta",
                        targetDbKey: "default",
                        externalSyncQuery: "external_query_1",
                        mergeFunctionName: "public.merge_function_1",
                        mergeParameterName: "p_entities",
                        needConvertDateTimeToUtc: true),
                    new MergeInfo<MergeResult>(
                        sourceDbKey: "axapta",
                        targetDbKey: "default",
                        externalSyncQuery: "external_query_2",
                        mergeFunctionName: "public.merge_function_2",
                        mergeParameterName: "p_entities",
                        needConvertDateTimeToUtc: false),
                    new MergeInfo<MergeResult>(
                        sourceDbKey: "axapta",
                        targetDbKey: "default",
                        externalSyncQuery: "external_query_3",
                        mergeFunctionName: "public.merge_function_3",
                        mergeParameterName: "p_entities",
                        needConvertDateTimeToUtc: false),
                    CancellationToken.None)
                .ConfigureAwait(false);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(mergeInfosCount));
            Assert.That(result.Select(r => r.Value), Is.All.EqualTo(scope.MergeResult.Value));
        }
    }
}
