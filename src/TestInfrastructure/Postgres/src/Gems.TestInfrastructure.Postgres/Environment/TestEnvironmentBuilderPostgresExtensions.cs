using DotNet.Testcontainers.Containers;

using Gems.TestInfrastructure.Environment;

using Testcontainers.PostgreSql;

using IDatabaseContainer = Gems.TestInfrastructure.Environment.IDatabaseContainer;

namespace Gems.TestInfrastructure.Postgres.Environment
{
    public static class TestEnvironmentBuilderPostgresExtensions
    {
        public static ITestEnvironmentBuilder UsePostgres(
            this ITestEnvironmentBuilder builder,
            string name,
            Func<PostgreSqlContainer, CancellationToken, Task> setupDatabase = default)
        {
            return builder.UsePostgres(name, (Action<PostgreSqlBuilder>)null, setupDatabase);
        }

        public static ITestEnvironmentBuilder UsePostgres(
            this ITestEnvironmentBuilder builder,
            string name)
        {
            return builder.UsePostgres(name, (Action<PostgreSqlBuilder>)null, null);
        }

        public static ITestEnvironmentBuilder UsePostgres(
            this ITestEnvironmentBuilder builder,
            string name,
            string image,
            Func<PostgreSqlContainer, CancellationToken, Task> setupDatabase = default)
        {
            return builder.UsePostgres(name, image, null, setupDatabase);
        }

        public static ITestEnvironmentBuilder UsePostgres(
            this ITestEnvironmentBuilder builder,
            string name,
            Action<PostgreSqlBuilder> setupContainer = default,
            Func<PostgreSqlContainer, CancellationToken, Task> setupDatabase = default)
        {
            return builder.UseComponent(() =>
            {
                var postgresBuilder = new PostgreSqlBuilder();
                setupContainer?.Invoke(postgresBuilder);
                var container = postgresBuilder.Build();
                builder.UseBootstraper(async (env, ct) =>
                {
                    await container.StartAsync();
                    env.RegisterComponent(name, container, typeof(PostgreSqlContainer), typeof(DockerContainer));
                    env.RegisterComponent<IDatabaseContainer>(name, new PostgresDatabaseContainer(container));
                    if (setupDatabase != null)
                    {
                        await setupDatabase?.Invoke(container, ct);
                    }
                });
                return container;
            });
        }

        public static ITestEnvironmentBuilder UsePostgres(
            this ITestEnvironmentBuilder builder,
            string name,
            string image,
            Action<PostgreSqlBuilder> setupContainer = default,
            Func<PostgreSqlContainer, CancellationToken, Task> setupDatabase = default)
        {
            return builder.UsePostgres(
                name,
                postgreSqlBuilder =>
                {
                    postgreSqlBuilder.WithImage(image);
                    setupContainer?.Invoke(postgreSqlBuilder);
                },
                setupDatabase);
        }
    }
}
