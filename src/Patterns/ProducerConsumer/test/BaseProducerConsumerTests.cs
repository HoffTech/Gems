// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using NUnit.Framework;

namespace Gems.Patterns.ProducerConsumer.Tests
{
    public class BaseProducerConsumerTests
    {
        [Test]
        public async Task TestSuccessfulProduceConsume()
        {
            // Arrange
            var cts = new CancellationTokenSource();

            var options = new ProducerConsumerOptions
            {
                DelayBetweenAttemptsInMilliseconds = 30,
                MaxAttempts = 3
            };

            const int elementsCount = 10000;

            var outputElementsList = new List<int>();

            var testIntConsumer = new ProducerConsumer<List<int>>(
                options: Options.Create(options),
                produceInternalAsync: async (pc, cancellationToken) =>
                {
                    var taskInfo = new List<int>();
                    for (var i = 0; i < elementsCount; i++)
                    {
                        taskInfo.Add(i);
                        if (taskInfo.Count == 1000)
                        {
                            pc.AddTaskInfo(taskInfo);
                            await Task.Delay(1, cancellationToken).ConfigureAwait(false);
                            taskInfo = new List<int>();
                        }
                    }
                },
                consumeInternalAsync: (taskInfo, _) =>
                {
                    outputElementsList.AddRange(taskInfo);
                    return Task.CompletedTask;
                });

            // Act
            await testIntConsumer.StartAsync(cts.Token).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(elementsCount, outputElementsList.Count);
        }

        [Test]
        public void TestConsumerFailed()
        {
            // Arrange
            var cts = new CancellationTokenSource();

            var options = new ProducerConsumerOptions
            {
                DelayBetweenAttemptsInMilliseconds = 30,
                MaxAttempts = 3
            };

            const int elementsCount = 10000;

            var outputElementsList = new List<int>();

            var testIntConsumer = new ProducerConsumer<List<int>>(
                options: Options.Create(options),
                produceInternalAsync: async (pc, cancellationToken) =>
                {
                    var taskInfo = new List<int>();
                    for (var i = 0; i < elementsCount; i++)
                    {
                        taskInfo.Add(i);
                        if (taskInfo.Count == 1000)
                        {
                            pc.AddTaskInfo(taskInfo);
                            await Task.Delay(1, cancellationToken).ConfigureAwait(false);
                            taskInfo = new List<int>();
                        }
                    }
                },
                consumeInternalAsync: (taskInfo, _) =>
                {
                    outputElementsList.AddRange(taskInfo);
                    if (taskInfo.Contains(2000))
                    {
                        throw new Exception("Some exception");
                    }

                    return Task.CompletedTask;
                });

            // Act
            async Task Act() => await testIntConsumer.StartAsync(cts.Token).ConfigureAwait(false);

            // Assert
            Assert.ThrowsAsync<Exception>(Act);
            Assert.AreNotEqual(elementsCount, outputElementsList.Count);
        }
    }
}
