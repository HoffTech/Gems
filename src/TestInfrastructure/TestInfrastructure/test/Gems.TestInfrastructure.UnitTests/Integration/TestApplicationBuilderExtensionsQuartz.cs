using FluentAssertions;

using Gems.TestInfrastructure.Integration;
using Gems.TestInfrastructure.Quartz.Integration;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Gems.TestInfrastructure.UnitTests.Integration
{
    public partial class TestApplicationBuilderExtensionsQuartz
    {
        [Test]
        public void RemoveQuartzHostedService()
        {
            DoTest(
                services => services.AddSingleton<QuartzHostedService>(),
                builder => builder.RemoveQuartzHostedService(),
                (builder, services) =>
                {
                    services
                        .Should()
                        .HaveCount(0);
                });
        }

        private static ITestApplicationBuilder DoTest(
            Action<IServiceCollection> arrange,
            Action<ITestApplicationBuilder> act,
            Action<Mock<ITestApplicationBuilder>, IServiceCollection> assert)
        {
            var services = new ServiceCollection();
            var mockBuilder = new Mock<ITestApplicationBuilder>();
            mockBuilder
                .Setup(x => x.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
                .Callback(new InvocationAction(i =>
                {
                    var action = i.Arguments.First() as Action<IServiceCollection>;
                    action!.Invoke(services);
                }))
                .Returns(() => mockBuilder.Object);
            var builder = mockBuilder.Object;
            act(builder);
            assert(mockBuilder, services);
            return builder;
        }
    }
}
