using Gems.TestInfrastructure.Integration;

using Moq;

namespace Gems.TestInfrastructure.UnitTests.Integration
{
    public class TestApplicationBuilderExtensionsUseEnvironment
    {
        [TestCase("Development")]
        [TestCase("Staging")]
        [TestCase("Production")]
        public void StringArgument(string environment)
        {
            DoTest(
                builder => builder.UseEnvironment(environment),
                builder => builder.Verify(x => x.UseEnvironment(It.Is(environment, StringComparer.Ordinal))));
        }

        [TestCase(ConfigurationEnvironment.Development, "Development")]
        [TestCase(ConfigurationEnvironment.Staging, "Staging")]
        [TestCase(ConfigurationEnvironment.Production, "Production")]
        public void ConfigurationEnvironmentArgument(
            ConfigurationEnvironment environment,
            string expected)
        {
            DoTest(
                builder => builder.UseEnvironment(environment),
                builder => builder.Verify(x => x.UseEnvironment(It.Is(expected, StringComparer.Ordinal))));
        }

        private static ITestApplicationBuilder DoTest(
            Action<ITestApplicationBuilder> act,
            Action<Mock<ITestApplicationBuilder>> assert)
        {
            var mockBuilder = new Mock<ITestApplicationBuilder>();
            var builder = mockBuilder.Object;
            act(builder);
            assert(mockBuilder);
            return builder;
        }
    }
}
