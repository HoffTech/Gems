// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

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
            return builder.UsePostgres(name, (Func<PostgreSqlBuilder, PostgreSqlBuilder>)null, setupDatabase);
        }

        public static ITestEnvironmentBuilder UsePostgres(
            this ITestEnvironmentBuilder builder,
            string name)
        {
            return builder.UsePostgres(name, (Func<PostgreSqlBuilder, PostgreSqlBuilder>)null, null);
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
            Func<PostgreSqlBuilder, PostgreSqlBuilder> setupContainer = default,
            Func<PostgreSqlContainer, CancellationToken, Task> setupDatabase = default)
        {
            return builder.UseComponent(() =>
            {
                var postgresBuilder = new PostgreSqlBuilder();
                postgresBuilder = setupContainer?.Invoke(postgresBuilder) ?? postgresBuilder;
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
            Func<PostgreSqlBuilder, PostgreSqlBuilder> setupContainer = default,
            Func<PostgreSqlContainer, CancellationToken, Task> setupDatabase = default)
        {
            return builder.UsePostgres(
                name,
                postgreSqlBuilder =>
                {
                    postgreSqlBuilder = postgreSqlBuilder.WithImage(image);
                    return setupContainer?.Invoke(postgreSqlBuilder) ?? postgreSqlBuilder;
                },
                setupDatabase);
        }
    }
}
