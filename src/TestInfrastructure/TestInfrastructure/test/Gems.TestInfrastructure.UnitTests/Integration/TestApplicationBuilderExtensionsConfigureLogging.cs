using Gems.TestInfrastructure.Integration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace Gems.TestInfrastructure.UnitTests.Integration
{
    public class TestApplicationBuilderExtensionsConfigureLogging
    {
        [Test]
        public void LogClearProviders()
        {
            DoTest(
                builder => builder.LogClearProviders(),
                (builder, loggingBuilder, services) =>
                {
                    builder.Verify(x => x.ConfigureLogging(It.IsAny<Action<ILoggingBuilder>>()));
                    builder.VerifyNoOtherCalls();
                    loggingBuilder.VerifyGet(x => x.Services);
                    services.VerifyGet(x => x.Count);
                });
        }

        [Test]
        public void LogToConsole()
        {
            DoTest(
                builder => builder.LogToConsole(),
                (builder, loggingBuilder, services) =>
                {
                    builder.Verify(x => x.ConfigureLogging(It.IsAny<Action<ILoggingBuilder>>()));
                    builder.VerifyNoOtherCalls();
                    loggingBuilder.VerifyGet(x => x.Services);
                    services.Verify(x => x.Add(It.IsAny<ServiceDescriptor>()));
                });
        }

        [Test]
        public void LogToDebug()
        {
            DoTest(
                builder => builder.LogToDebug(),
                (builder, loggingBuilder, services) =>
                {
                    builder.Verify(x => x.ConfigureLogging(It.IsAny<Action<ILoggingBuilder>>()));
                    builder.VerifyNoOtherCalls();
                    loggingBuilder.VerifyGet(x => x.Services);
                    services.Verify(x => x.Add(It.IsAny<ServiceDescriptor>()));
                });
        }

        [Test]
        public void ConfigureLogging()
        {
            DoTest(
                builder => builder.ConfigureLogging(l => l.AddDebug()),
                (builder, loggingBuilder, services) =>
                {
                    builder.Verify(x => x.ConfigureLogging(It.IsAny<Action<ILoggingBuilder>>()));
                    builder.VerifyNoOtherCalls();
                    loggingBuilder.VerifyGet(x => x.Services);
                    services.Verify(x => x.Add(It.IsAny<ServiceDescriptor>()));
                });
        }

        [TestCase(LogLevel.None)]
        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Information)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Warning)]
        public void LogSetMinimumLevel(LogLevel logLevel)
        {
            DoTest(
                builder => builder.LogSetMinimumLevel(logLevel),
                (builder, loggingBuilder, services) =>
                {
                    builder.Verify(x => x.ConfigureLogging(It.IsAny<Action<ILoggingBuilder>>()));
                    builder.VerifyNoOtherCalls();
                    loggingBuilder.VerifyGet(x => x.Services);
                    services.Verify(x => x.Add(It.IsAny<ServiceDescriptor>()));
                });
        }

        private static void DoTest(
            Action<ITestApplicationBuilder> act,
            Action<Mock<ITestApplicationBuilder>, Mock<ILoggingBuilder>, Mock<IServiceCollection>> assert)
        {
            var mockServicesCollection = new Mock<IServiceCollection>();
            var services = mockServicesCollection.Object;
            var mockLoggingBuilder = new Mock<ILoggingBuilder>();
            mockLoggingBuilder
                .SetupGet(x => x.Services)
                .Returns(services);
            var loggingBuilder = mockLoggingBuilder.Object;

            var mockBuilder = new Mock<ITestApplicationBuilder>();
            mockBuilder
                .Setup(x => x.ConfigureLogging(It.IsAny<Action<ILoggingBuilder>>()))
                .Callback(new InvocationAction(i =>
                {
                    var action = i.Arguments.First() as Action<ILoggingBuilder>;
                    action!.Invoke(loggingBuilder);
                }))
                .Returns(() => mockBuilder.Object);

            var builder = mockBuilder.Object;
            act(builder);

            assert(mockBuilder, mockLoggingBuilder, mockServicesCollection);
        }
    }
}
