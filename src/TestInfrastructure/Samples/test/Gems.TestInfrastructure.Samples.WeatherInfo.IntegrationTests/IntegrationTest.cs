using System.Net;

using Dapper;

using FluentAssertions;

using FluentHttpClient;

using Gems.TestInfrastructure.Assertion;
using Gems.TestInfrastructure.Assertion.Database;
using Gems.TestInfrastructure.Environment;
using Gems.TestInfrastructure.Integration;
using Gems.TestInfrastructure.Postgres.Environment;
using Gems.TestInfrastructure.Postgres.Utils.Database;
using Gems.TestInfrastructure.Utils.Database;
using Gems.TestInfrastructure.Utils.Http;
using Gems.TestInfrastructure.WireMock.Environment;

using Microsoft.Extensions.Logging;

using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Gems.TestInfrastructure.Samples.WeatherInfo.IntegrationTests
{
    public class IntegrationTest
    {
        private ITestEnvironment env;
        private ITestApplication app;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            this.env = await new TestEnvironmentBuilder()
                .UsePostgres(
                    "DefaultConnection",
                    async (c, ct) =>
                    {
                        await c.ExecScriptAsync(new FileInfo(@"Resources/Sql/migration.sql"), ct);
                        await c.ExecScriptAsync(new FileInfo(@"Resources/Sql/data.sql"), ct);
                        await c.SetupAsync(
                            async (connection, schema) =>
                            {
                                await connection.ImportCsvFileAsync(
                                    schema.CurrentDatabase.Table("precipitations"),
                                    new FileInfo(@"Resources/Csv/precipitations.csv"));
                            },
                            ct);
                    })
                .UseWireMockServer(
                    "Default",
                    server =>
                    {
                        server
                            .Given(Request.Create()
                                .WithPath("/TemperatureInfo/*")
                                .UsingGet())
                            .RespondWith(Response.Create()
                                .WithBody(@"{ ""temperature"": ""10.0"" }"));

                        server
                            .Given(Request.Create()
                                .WithPath("/PrecipitationInfo/*")
                                .UsingGet())
                            .RespondWith(Response.Create()
                                .WithBody(@"{ ""precipitation"": ""Snow"" }"));
                    })
                .BuildAsync();

            this.app = new TestApplicationBuilder<Program>()
                .UseEnvironment(ConfigurationEnvironment.Development)
                .UseConnectionString("TemperatureInfo", UriExtensions.Add(this.env.WireMockServerUri("Default"), "TemperatureInfo/"))
                .UseConnectionString("PrecipitationInfo", UriExtensions.Add(this.env.WireMockServerUri("Default"), "PrecipitationInfo/"))
                .UseConnectionString("DefaultConnection", this.env)
                .LogToConsole()
                .LogSetMinimumLevel(LogLevel.Trace)
                .Build();
        }

        [OneTimeTearDown]
        public void CleanupAsync()
        {
            this.app?.Dispose();
            this.env?.Dispose();
        }

        [Test]
        public async Task GetRoot_Success()
        {
            using var actualResponse = await this.app.HttpClient
                .UsingRoute("/")
                .WithQueryParam("mode", "sync")
                .GetAsync();

            actualResponse
                .Should()
                .NotBeNull();
            actualResponse.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            var actual = await actualResponse.GetResponseStringAsync();

            actual
                .Should()
                .AsJson()
                .BeEquivalentTo("{\"items\":[{\"town\":\"Москва\",\"temperature\":\"10.0\",\"precipitation\":\"Snow\"},{\"town\":\"Санкт-Петербург\",\"temperature\":\"10.0\",\"precipitation\":\"Snow\"},{\"town\":\"Екатеринбург\",\"temperature\":\"10.0\",\"precipitation\":\"Snow\"},{\"town\":\"Новосибирск\",\"temperature\":\"10.0\",\"precipitation\":\"Snow\"},{\"town\":\"Сочи\",\"temperature\":\"10.0\",\"precipitation\":\"Snow\"}]}");
        }

        [Test]
        public async Task DatabaseSchemaValid()
        {
            await using var connection = await this.env.Database("DefaultConnection").ConnectPostgresAsync();

            var schema = await connection.SchemaAsync();

            schema.Users.Should().Contain("postgres");
            schema.Databases.Should().Contain("postgres");
            var db = schema.Databases.First();

            db.Tables
                .Should()
                .Contain("towns")
                .Which
                .Columns.Should().Contain("town_name");

            db.Table("towns")
                .Indexes
                .Should()
                .Contain("towns_pk")
                .Which
                .Columns.Should().Contain("town_name");

            db.Tables
                .Should()
                .Contain("public.temperatures");

            db.Tables
                .Should()
                .Contain("public.precipitations");

            var precipitations = (await connection.QueryAsync(@"SELECT * FROM public.precipitations;")).ToList();
            precipitations
                .Any(x => x.town_name.ToString() == "Москва")
                .Should()
                .BeTrue();
        }
    }
}
