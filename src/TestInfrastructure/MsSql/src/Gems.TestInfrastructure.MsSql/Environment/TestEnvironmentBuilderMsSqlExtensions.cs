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
        Func<MsSqlContainer, CancellationToken, Task> setupDatabase = default)
    {
        return builder.UseMsSql(name, (Action<MsSqlBuilder>)null, setupDatabase);
    }

    public static ITestEnvironmentBuilder UseMsSql(
        this ITestEnvironmentBuilder builder,
        string name)
    {
        return builder.UseMsSql(name, (Action<MsSqlBuilder>)null, null);
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
        Action<MsSqlBuilder> setupContainer = default,
        Func<MsSqlContainer, CancellationToken, Task> setupDatabase = default)
    {
        return builder.UseComponent(() =>
        {
            var mssqlBuilder = new MsSqlBuilder();
            setupContainer?.Invoke(mssqlBuilder);
            var container = mssqlBuilder.Build();
            builder.UseBootstraper(async (env, ct) =>
            {
                await container.StartAsync();
                env.RegisterComponent(name, container, typeof(MsSqlContainer), typeof(DockerContainer));
                env.RegisterComponent<IDatabaseContainer>(name, new MsSqlDatabaseContainer(container));
                await setupDatabase?.Invoke(container, ct);
            });
            return container;
        });
    }

    public static ITestEnvironmentBuilder UseMsSql(
        this ITestEnvironmentBuilder builder,
        string name,
        string image,
        Action<MsSqlBuilder> setupContainer = default,
        Func<MsSqlContainer, CancellationToken, Task> setupDatabase = default)
    {
        return builder.UseMsSql(
            name,
            msSqlBuilder =>
            {
                msSqlBuilder.WithImage(image);
                setupContainer?.Invoke(msSqlBuilder);
            },
            setupDatabase);
    }
}
