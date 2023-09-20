// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Tasks.Tests.Exceptions;
using Gems.Tasks.Tests.FakeServices;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

namespace Gems.Tasks.Tests
{
    [TestFixture]
    public class ParallelExecutorTests
    {
        private IServiceProvider serviceProvider;
        private List<int> sourceDataList;
        private ConcurrentBag<int> syncedDataBag;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped(_ =>
            {
                var callCount = 0;
                var mock = new Mock<IFakeService>();
                mock.Setup(m => m.DoFakeWorkAsync(CancellationToken.None))
                    .Callback(() =>
                    {
                        callCount++;
                        switch (callCount)
                        {
                            case <= 5:
                                throw new TestException();
                        }
                    })
                    .Returns(Task.CompletedTask);

                return mock.Object;
            });

            this.serviceProvider = services.BuildServiceProvider();
            this.syncedDataBag = new ConcurrentBag<int>();
        }

        [Test]
        [TestCase(12, 4, 1)]
        [TestCase(1_000_000, 15, 1000)]
        [TestCase(3_714_238, 30, 3000)]
        [TestCase(3_714_238, 100, 1000)]
        [TestCase(3_000_001, 30, 5000)]
        [TestCase(1_000_001, 15, 500)]
        [TestCase(2_000_001, 50, 1000)]
        [TestCase(3_714_238, 15, 500)]
        [TestCase(3_714_238, 100, 10000)]
        public async Task ParallelExecuteAsync_NoErrors_DataIsSynchronized(
            int totalRows,
            int maxSemaphoreTasks,
            int maxTakeSize)
        {
            // Arrange
            using var tokenSource = new CancellationTokenSource();
            this.sourceDataList = CreateSourceDataList(totalRows).ToList();

            // Act
            await ParallelExecutor.SyncDataAsync(
                totalRows,
                maxSemaphoreTasks,
                new List<Type> { typeof(TestException) },
                this.ReadAndWritePartOfDataAsync,
                tokenSource.Token,
                maxTakeSize);

            var isSynced = new HashSet<int>(this.sourceDataList).SetEquals(this.syncedDataBag);

            // Assert
            Assert.IsNotNull(this.syncedDataBag);
            Assert.IsNotEmpty(this.syncedDataBag);
            Assert.AreEqual(totalRows, this.syncedDataBag.Count);
            Assert.IsTrue(isSynced);
            Assert.Pass();
        }

        [Test]
        [TestCase(12, 4, 1, 10)]
        [TestCase(1_000_000, 15, 1000, 10)]
        [TestCase(3_714_238, 30, 3000, 10)]
        [TestCase(3_714_238, 100, 1000, 10)]
        [TestCase(3_000_001, 30, 5000, 10)]
        [TestCase(1_000_001, 15, 500, 10)]
        [TestCase(2_000_001, 50, 1000, 10)]
        [TestCase(3_714_238, 15, 500, 10)]
        [TestCase(3_714_238, 100, 10000, 10)]
        public async Task ParallelExecuteAsync_ThrowsSqlExceptionsButHandleIt_DataIsSynchronized(
            int totalRows,
            int maxSemaphoreTasks,
            int maxTakeSize,
            int maxAttempts)
        {
            // Arrange
            using var tokenSource = new CancellationTokenSource();
            this.sourceDataList = CreateSourceDataList(totalRows).ToList();

            // Act
            await ParallelExecutor.SyncDataAsync(
                totalRows,
                maxSemaphoreTasks,
                new List<Type> { typeof(TestException) },
                this.ReadAndWritePartOfDataWithExceptionsAsync,
                tokenSource.Token,
                maxTakeSize,
                maxAttempts);

            var isSynced = new HashSet<int>(this.sourceDataList).SetEquals(this.syncedDataBag);

            // Assert
            Assert.IsNotNull(this.syncedDataBag);
            Assert.IsNotEmpty(this.syncedDataBag);
            Assert.AreEqual(totalRows, this.syncedDataBag.Count);
            Assert.IsTrue(isSynced);
            Assert.Pass();
        }

        private static IEnumerable<int> CreateSourceDataList(int totalRows)
        {
            return Enumerable.Range(0, totalRows);
        }

        private Task ReadAndWritePartOfDataAsync(int skip, int take, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"skip: {skip}\ttake: {take}\tCurrentThread: {Environment.CurrentManagedThreadId}");

            var data = this.sourceDataList.GetRange(skip, take);
            foreach (var item in data)
            {
                this.syncedDataBag.Add(item);
            }

            return Task.CompletedTask;
        }

        private async Task ReadAndWritePartOfDataWithExceptionsAsync(int skip, int take, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"skip: {skip}\ttake: {take}\tCurrentThread: {Environment.CurrentManagedThreadId}");

            var fakeService = this.serviceProvider.GetRequiredService<IFakeService>();
            await fakeService.DoFakeWorkAsync(CancellationToken.None);

            await this.ReadAndWritePartOfDataAsync(skip, take, cancellationToken);
        }
    }
}
