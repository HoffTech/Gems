using Gems.TestInfrastructure.Integration;

namespace Gems.TestInfrastructure.Quartz.Integration
{
    public static class TestApplicationBuilderQuartzExtensions
    {
        public static ITestApplicationBuilder RemoveQuartzHostedService(this ITestApplicationBuilder builder)
        {
            return builder.RemoveServiceImplementationByName("QuartzHostedService");
        }
    }
}
