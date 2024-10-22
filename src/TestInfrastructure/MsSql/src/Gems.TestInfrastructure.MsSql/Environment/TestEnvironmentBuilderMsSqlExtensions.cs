// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using DotNet.Testcontainers.Containers;

using Gems.TestInfrastructure.Environment;

using Testcontainers.MsSql;

using IDatabaseContainer = Gems.TestInfrastructure.Environment.IDatabaseContainer;

namespace Gems.TestInfrastructure.MsSql.Environment;

public static class TestEnvironmentBuilderMsSqlExtensions
{
    public static ITestEnvironmentBuilder UseMsSql(
        this ITestEnvironmentBuilder builder,
        string name,
        Func<MsSqlContainer, CancellationToken, Task> setupDatabase)
    {
        return builder.UseMsSql(name, (Func<MsSqlBuilder, MsSqlBuilder>)null, setupDatabase);
    }

    public static ITestEnvironmentBuilder UseMsSql(
        this ITestEnvironmentBuilder builder,
        string name)
    {
        return builder.UseMsSql(name, (Func<MsSqlBuilder, MsSqlBuilder>)null);
    }

    public static ITestEnvironmentBuilder UseMsSql(
        this ITestEnvironmentBuilder builder,
        string name,
        string image,
        Func<MsSqlContainer, CancellationToken, Task> setupDatabase = default)
    {
        return builder.UseMsSql(name, image, null, setupDatabase);
    }

    public static ITestEnvironmentBuilder UseMsSql(
        this ITestEnvironmentBuilder builder,
        string name,
        Func<MsSqlBuilder, MsSqlBuilder> setupContainer,
        Func<MsSqlContainer, CancellationToken, Task> setupDatabase = default)
    {
        return builder.UseComponent(() =>
        {
            var msSqlBuilder = new MsSqlBuilder();
            msSqlBuilder = setupContainer?.Invoke(msSqlBuilder) ?? msSqlBuilder;
            var container = msSqlBuilder.Build();
            builder.UseBootstraper(async (env, ct) =>
            {
                await container.StartAsync();
                env.RegisterComponent(name, container, typeof(MsSqlContainer), typeof(DockerContainer));
                env.RegisterComponent<IDatabaseContainer>(name, new MsSqlDatabaseContainer(container));
                if (setupDatabase != null)
                {
                    await setupDatabase.Invoke(container, ct);
                }
            });
            return container;
        });
    }

    public static ITestEnvironmentBuilder UseMsSql(
        this ITestEnvironmentBuilder builder,
        string name,
        string image,
        Func<MsSqlBuilder, MsSqlBuilder> setupContainer = default,
        Func<MsSqlContainer, CancellationToken, Task> setupDatabase = default)
    {
        return builder.UseMsSql(
            name,
            msSqlBuilder =>
            {
                msSqlBuilder = msSqlBuilder.WithImage(image);
                return setupContainer?.Invoke(msSqlBuilder) ?? msSqlBuilder;
            },
            setupDatabase);
    }
}
