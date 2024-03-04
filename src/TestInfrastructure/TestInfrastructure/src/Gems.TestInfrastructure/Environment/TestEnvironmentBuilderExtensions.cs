using DotNet.Testcontainers.Containers;

namespace Gems.TestInfrastructure.Environment
{
    public static class TestEnvironmentBuilderExtensions
    {
        public static ITestEnvironmentBuilder UseDockerContainer<TContainer>(
            this ITestEnvironmentBuilder builder,
            string name,
            Func<TContainer> factory,
            Func<TContainer, CancellationToken, Task> containerSetup = default)
            where TContainer : DockerContainer
        {
            return builder.UseComponent(() =>
            {
                var container = factory();
                builder.UseBootstraper(async (env, ct) =>
                {
                    await container.StartAsync();
                    env.RegisterComponent(name, container);
                    await containerSetup?.Invoke(container, ct);
                });
                return container;
            });
        }
    }
}
