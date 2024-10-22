// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Dapper;

using FluentAssertions;

using Gems.TestInfrastructure.Assertion.Database;
using Gems.TestInfrastructure.Environment;
using Gems.TestInfrastructure.Postgres.Environment;
using Gems.TestInfrastructure.Postgres.Utils.Database;

namespace Gems.TestInfrastructure.Samples.EfData.IntegrationTests
{
    public class Tests
    {
        private const string ContainerName = "MyPostgres";

        [Test]
        public async Task MigrateFromPostgreSqlContainerAsync()
        {
            await using var env = await new TestEnvironmentBuilder()
                .UsePostgres(
                    ContainerName,
                    (c, ct) => PostgresEfMigrationExtension.MigrateAsync<Context>(c, ct))
                .BuildAsync();
            await using var connection = await env.ConnectPostgresAsync(ContainerName);
            var schema = await connection.SchemaAsync();
            var db = schema.CurrentDatabase;

            var result = await connection.QueryAsync("SELECT * FROM \"__EFMigrationsHistory\";");
            db.Tables
                .Should()
                .Contain("products")
                .Which
                .Indexes
                    .Should()
                    .Contain("ix_product_product_category_id");

            db.Tables
                .Should()
                .Contain("product_categories");
        }

        [Test]
        public async Task MigrateFromTestEnvironmentAsync()
        {
            await using var env = await new TestEnvironmentBuilder()
               .UsePostgres(ContainerName)
               .BuildAsync();
            await env.MigrateAsync<Context>(ContainerName);
            await using var connection = await env.ConnectPostgresAsync(ContainerName);
            var schema = await connection.SchemaAsync();
            var db = schema.CurrentDatabase;

            db.Tables
                .Should()
                .Contain("products")
                .Which
                .Indexes
                    .Should()
                    .Contain("ix_product_product_category_id");

            db.Tables
                .Should()
                .Contain("product_categories");
        }
    }
}
