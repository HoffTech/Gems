// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

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
