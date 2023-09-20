// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Tasks.Tests.Exceptions;
using Gems.Tasks.Tests.FakeServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using NUnit.Framework;

namespace Gems.Tasks.Tests
{
    [TestFixture]
    public class AsyncDecoratorTests
    {
        private IServiceProvider serviceProvider;

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
            services.AddSingleton<ILogger<AsyncDecoratorTests>, NullLogger<AsyncDecoratorTests>>();
            this.serviceProvider = services.BuildServiceProvider();
        }

        [Test]
        public async Task DurableExecuteAsync_AttemptsMoreThenErrors_ShouldSuccess()
        {
            // Arrange
            var fakeService = this.serviceProvider.GetRequiredService<IFakeService>();
            var logger = this.serviceProvider.GetRequiredService<ILogger<AsyncDecoratorTests>>();

            // Act
            var result = await AsyncDecorator
                .DurableExecuteAsync(
                    async (cancellationToken) =>
                    {
                        await fakeService.DoFakeWorkAsync(cancellationToken).ConfigureAwait(false);
                        return Task.CompletedTask;
                    },
                    CancellationToken.None,
                    TimeSpan.FromMilliseconds(500),
                    maxAttempts: 10,
                    new List<Type> { typeof(TestException) },
                    ex =>
                    {
                        logger.LogError(ex, "Test error log");
                        return Task.CompletedTask;
                    })
                .ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.Pass();
        }

        [Test]
        public void DurableExecuteAsync_AttemptsLessThenErrors_ShouldThrowTestException()
        {
            // Arrange
            var fakeService = this.serviceProvider.GetRequiredService<IFakeService>();
            var logger = this.serviceProvider.GetRequiredService<ILogger<AsyncDecoratorTests>>();

            // Act
            async Task ExecuteMethodAsync()
            {
                await AsyncDecorator.DurableExecuteAsync(
                        async (cancellationToken) =>
                        {
                            await fakeService.DoFakeWorkAsync(cancellationToken).ConfigureAwait(false);
                            return Task.CompletedTask;
                        },
                        CancellationToken.None,
                        TimeSpan.FromMilliseconds(500),
                        maxAttempts: 3,
                        new List<Type> { typeof(TestException) },
                        ex =>
                        {
                            logger.LogError(ex, "Test error log");
                            return Task.CompletedTask;
                        })
                    .ConfigureAwait(false);
            }

            // Assert
            Assert.ThrowsAsync<TestException>(ExecuteMethodAsync);
            Assert.Pass();
        }
    }
}