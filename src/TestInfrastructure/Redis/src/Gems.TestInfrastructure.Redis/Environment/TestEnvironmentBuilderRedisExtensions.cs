using DotNet.Testcontainers.Containers;

using Gems.TestInfrastructure.Environment;

using Testcontainers.Redis;

using IDatabaseContainer = Gems.TestInfrastructure.Environment.IDatabaseContainer;

namespace Gems.TestInfrastructure.Redis.Environment;

public static class TestEnvironmentBuilderRedisExtensions
{
    public static ITestEnvironmentBuilder UseRedis(
        this ITestEnvironmentBuilder builder,
        string name,
        Func<RedisContainer, CancellationToken, Task> setupDatabase = default)
    {
        return builder.UseRedis(name, (Action<RedisBuilder>)null, setupDatabase);
    }

    public static ITestEnvironmentBuilder UseRedis(
        this ITestEnvironmentBuilder builder,
        string name)
    {
        return builder.UseRedis(name, (Action<RedisBuilder>)null, null);
    }

    public static ITestEnvironmentBuilder UseRedis(
        this ITestEnvironmentBuilder builder,
        string name,
        string image,
        Func<RedisContainer, CancellationToken, Task> setupDatabase = default)
    {
        return builder.UseRedis(name, image, null, setupDatabase);
    }

    public static ITestEnvironmentBuilder UseRedis(
        this ITestEnvironmentBuilder builder,
        string name,
        Action<RedisBuilder> setupContainer = default,
        Func<RedisContainer, CancellationToken, Task> setupDatabase = default)
    {
        return builder.UseComponent(() =>
        {
            var redisBuilder = new RedisBuilder();
            setupContainer?.Invoke(redisBuilder);
            var container = redisBuilder.Build();
            builder.UseBootstraper(async (env, ct) =>
            {
                await container.StartAsync();
                env.RegisterComponent(name, container, typeof(RedisContainer), typeof(DockerContainer));
                env.RegisterComponent<IDatabaseContainer>(name, new RedisDatabaseContainer(container));
                await setupDatabase?.Invoke(container, ct);
            });
            return container;
        });
    }

    public static ITestEnvironmentBuilder UseRedis(
        this ITestEnvironmentBuilder builder,
        string name,
        string image,
        Action<RedisBuilder> setupContainer = default,
        Func<RedisContainer, CancellationToken, Task> setupDatabase = default)
    {
        return builder.UseRedis(
            name,
            redisBuilder =>
            {
                redisBuilder.WithImage(image);
                setupContainer?.Invoke(redisBuilder);
            },
            setupDatabase);
    }
}
