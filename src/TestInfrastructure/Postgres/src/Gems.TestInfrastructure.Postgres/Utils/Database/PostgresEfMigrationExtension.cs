// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Environment;

using Microsoft.EntityFrameworkCore;

using Testcontainers.PostgreSql;

namespace Gems.TestInfrastructure.Postgres.Utils.Database
{
    public static class PostgresEfMigrationExtension
    {
        public static async Task<PostgreSqlContainer> MigrateAsync<TContext>(
            this PostgreSqlContainer contaner,
            CancellationToken cancellationToken = default)
            where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseNpgsql(contaner.GetConnectionString());

            var options = optionsBuilder.Options;
            var context = (TContext)Activator.CreateInstance(typeof(TContext), options);
            await context.Database.MigrateAsync(cancellationToken);
            return contaner;
        }

        public static async Task<ITestEnvironment> MigrateAsync<TContext>(
            this ITestEnvironment env,
            string name,
            CancellationToken cancellationToken = default)
            where TContext : DbContext
        {
            await MigrateAsync<TContext>(env.Component<PostgreSqlContainer>(name), cancellationToken);
            return env;
        }
    }
}
