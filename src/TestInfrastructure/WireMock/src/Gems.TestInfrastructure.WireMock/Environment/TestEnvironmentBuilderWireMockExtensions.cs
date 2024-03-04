using Gems.TestInfrastructure.Environment;

using WireMock.Server;
using WireMock.Settings;

namespace Gems.TestInfrastructure.WireMock.Environment
{
    public static class TestEnvironmentBuilderWireMockExtensions
    {
        public static ITestEnvironmentBuilder UseWireMockServer(
            this ITestEnvironmentBuilder builder,
            string name,
            Action<WireMockServer> setupServer) =>
            builder.UseWireMockServer(name, null, setupServer);

        public static ITestEnvironmentBuilder UseWireMockServer(
            this ITestEnvironmentBuilder builder,
            string name,
            WireMockServerSettings settings = default,
            Action<WireMockServer> setupServer = default)
        {
            return builder.UseComponent(() =>
            {
                var serverContainer = new WireMockServerContainer(settings ?? new WireMockServerSettings());
                builder.UseBootstraper(async (env, ct) =>
                {
                    await Task.Run(() =>
                    {
                        serverContainer.Start();
                        env.RegisterComponent<WireMockServer>(name, serverContainer.WireMockServer);
                        setupServer?.Invoke(serverContainer.WireMockServer);
                    });
                });

                return serverContainer;
            });
        }
    }
}
