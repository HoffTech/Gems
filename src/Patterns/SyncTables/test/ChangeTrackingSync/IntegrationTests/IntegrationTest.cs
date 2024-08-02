// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using FluentAssertions;

using Gems.CompositionRoot;
using Gems.Patterns.SyncTables.ChangeTrackingSync;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Settings;
using Gems.Patterns.SyncTables.Options;
using Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.IntegrationTests.Entities;
using Gems.TestInfrastructure.Environment;
using Gems.TestInfrastructure.MsSql.Environment;
using Gems.TestInfrastructure.Postgres.Environment;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using Newtonsoft.Json;

using Npgsql;

using NUnit.Framework;

using Testcontainers.MsSql;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.IntegrationTests;

public class IntegrationTest
{
    public ITestEnvironment TestEvironment { get; set; }

    [OneTimeSetUp]
    public async Task InitializeEnvironment()
    {
        this.TestEvironment = await new TestEnvironmentBuilder()
            .UseMsSql(
                "source",
                async (c, ct) =>
                {
                    await c.ExecScriptAsync(new FileInfo(@"ChangeTrackingSync/IntegrationTests/Migrations/Source/beforeMigrate.sql"), ct);
                    await c.ExecScriptAsync(new FileInfo(@"ChangeTrackingSync/IntegrationTests/Migrations/Source/V0.01__Test_Ax_Source.sql"), ct);
                })
            .UsePostgres(
                "destination",
                async (c, ct) =>
                {
                    await c.ExecScriptAsync(new FileInfo(@"ChangeTrackingSync/IntegrationTests/Migrations/Destination/V0.01__Add_Sync_Information.sql"), ct);
                    await c.ExecScriptAsync(new FileInfo(@"ChangeTrackingSync/IntegrationTests/Migrations/Destination/V0.02__Add_Destination.sql"), ct);
                    await c.ExecScriptAsync(new FileInfo(@"ChangeTrackingSync/IntegrationTests/Migrations/Destination/R__Destination_Clear.sql"), ct);
                    await c.ExecScriptAsync(new FileInfo(@"ChangeTrackingSync/IntegrationTests/Migrations/Destination/R__Destination_Merge.sql"), ct);
                    await c.ExecScriptAsync(new FileInfo(@"ChangeTrackingSync/IntegrationTests/Migrations/Destination/R__SyncInfo_Get.sql"), ct);
                    await c.ExecScriptAsync(new FileInfo(@"ChangeTrackingSync/IntegrationTests/Migrations/Destination/R__SyncInfo_Upsert.sql"), ct);
                })
            .BuildAsync();
    }

    [OneTimeTearDown]
    public void CleanupEnvironmentAsync()
    {
        this.TestEvironment?.Dispose();
    }

    [Test]
    public async Task Emtpty_DB_Initial_And_Sync_Test()
    {
        var serviceProvider = this.BuildServiceProvider();

        var processor = serviceProvider
            .GetRequiredService<IChangeTrackingSyncTableProcessor<SourceEntity, DestinationEntity, DestinationSyncMergeResult>>();

        try
        {
            // initial full sync (on empty db)
            await processor.Sync(
                this.BuildSyncInfo(),
                new CancellationTokenSource().Token);

            await this.CheckSourceAndDestinationMatched();

            // initial ct sync (on empty db)
            await processor.Sync(
                this.BuildSyncInfo(),
                new CancellationTokenSource().Token);

            await this.CheckSourceAndDestinationMatched();
        }
        finally
        {
            await this.Cleanup();
        }
    }

    [Test]
    public async Task Db_With_Data_Initial_And_Sync_Test()
    {
        var serviceProvider = this.BuildServiceProvider();

        var processor = serviceProvider
            .GetRequiredService<IChangeTrackingSyncTableProcessor<SourceEntity, DestinationEntity, DestinationSyncMergeResult>>();

        try
        {
            await this.GetSourceConnection()
                .QueryAsync(
                    "INSERT INTO dbo.SourceData (ItemId, TextData, NumericData) VALUES('item1', 'data1', 123.123);");

            // initial full sync (have some data)
            await processor.Sync(
                this.BuildSyncInfo(),
                new CancellationTokenSource().Token);

            await this.CheckSourceAndDestinationMatched();

            // ct sync (no chnages)
            await processor.Sync(
                this.BuildSyncInfo(),
                new CancellationTokenSource().Token);

            await this.CheckSourceAndDestinationMatched();

            await this.GetSourceConnection()
                .QueryAsync(
                    "INSERT INTO dbo.SourceData (ItemId, TextData, NumericData) VALUES('item2', 'data2', 123.123);");

            // ct sync (new changes)
            await processor.Sync(
                this.BuildSyncInfo(),
                new CancellationTokenSource().Token);

            await this.CheckSourceAndDestinationMatched();
        }
        finally
        {
            await this.Cleanup();
        }
    }

    public ChangeTrackingSyncInfo BuildSyncInfo()
    {
        return new ChangeTrackingSyncInfo(
            new SourceDataSettings
            {
                DbKey = "mssql-ax-db",
                TableName = "dbo.SourceData",
                PrimaryKeyName = "RecId",
                BatchSize = 100_000,
                ChangesQuery =
@"WITH ChangedCTE
(
    [ChangeTrackingVersion],
    [OperationType],
    [RecId],
    [ItemId],
    [TextData],
    [NumericData]
)
AS
(
    select
        ct.SYS_CHANGE_VERSION [ChangeTrackingVersion],
        ct.SYS_CHANGE_OPERATION [OperationType],
        ct.[RecId],
        [ItemId],
        [TextData],
        [NumericData]
    from changetable(changes dbo.[SourceData], @version) as ct
    left join dbo.[SourceData] as it
        on ct.RECID = it.RECID
)

select top (@batchSize) with ties
    *
from  ChangedCTE WITH (FORCESEEK)
order by ChangeTrackingVersion
option(maxdop 1)",
                FullReloadQuery =
$@"select
	0 [ChangeTrackingVersion],
	'I' [OperationType],
	[RecId],
    [ItemId],
    [TextData],
    [NumericData]
from dbo.[SourceData]
where
    [RecId] >= @offset and [RecId] < @offset + @batchSize
order by [RecId]",
                GetCommandTimeout = 300,
                OnDestinationVersionOutdated = SyncErrorAction.FullReload,
                OnRestoreFromBackupDetected = SyncErrorAction.FullReload
            },
            new DestinationSettings
            {
                DbKey = "pg-destination-db",
                TableName = "public.destination",
                MergeFunctionName = "public.destination_merge",
                MergeParameterName = "changed_data",
                EnableFullChangesLog = false,
                ClearFunctionName = "public.destination_clear",
                MergeCommandTimeout = 600
            },
            needConvertDateTimeToUtc: true);
    }

    private string GetSourceConnectionString()
    {
        var connectionString = this.TestEvironment.DatabaseConnectionString("source");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var contanerHostName = this.TestEvironment.Component<MsSqlContainer>("source").Name.Replace("/", string.Empty);

            // for win tests
            connectionString = connectionString
                .Replace(contanerHostName, $"127.0.0.1\\{contanerHostName}")
                .Replace("localhost", $"127.0.0.1\\{contanerHostName}");
        }

        connectionString = connectionString
            .Replace("Database=master", "Database=Source");

        return connectionString;
    }

    private SqlConnection GetSourceConnection()
    {
        return new SqlConnection(this.GetSourceConnectionString());
    }

    private async Task Cleanup()
    {
        await this.GetSourceConnection()
            .QueryAsync("truncate table dbo.SourceData;");

        await new NpgsqlConnection(this.TestEvironment.DatabaseConnectionString("destination"))
            .QueryAsync(
            "truncate table public.destination;\n" +
            "truncate table public.sync_info");
    }

    private async Task CheckSourceAndDestinationMatched()
    {
        var sourceInitial = await this.GetSourceConnection()
            .QueryAsync<SourceEntity>("select * from dbo.SourceData");

        var destinationInitial = await new NpgsqlConnection(this.TestEvironment.DatabaseConnectionString("destination"))
            .QueryAsync<DestinationEntity>("select * from public.destination");

        sourceInitial.Should()
            .BeEquivalentTo(destinationInitial);
    }

    private ServiceProvider BuildServiceProvider()
    {
        var pgConfig =
            $@"{{
                ""PostgresqlUnitOfWorks"": [
                {{
                  ""Key"": ""pg-destination-db"",
                  ""Options"": {{
                    ""ConnectionString"": {JsonConvert.ToString(this.TestEvironment.DatabaseConnectionString("destination"))},
                    ""DbQueryMetricInfo"": {{
                      ""Name"": ""changes_source_db_query_time"",
                      ""Description"": ""Time of database query execution""
                    }},
                    ""SuspendTransaction"": true,
                    ""SuspendRegisterMappersFromAssemblyContaining"" : false
                  }}
                }}
              ]
            }}";

        var mssqlConfig =
            @$"{{
                ""MsSqlUnitOfWorks"": [
                    {{
                      ""Key"": ""mssql-ax-db"",
                      ""Options"": {{
                        ""ConnectionString"": {JsonConvert.ToString(this.GetSourceConnectionString())},
                        ""DbQueryMetricInfo"": {{
                          ""Name"": ""changes_source_db_query_time"",
                          ""Description"": ""Time of database query execution""
                        }},
                        ""SuspendTransaction"": true
                      }}
                    }}
                ]
            }}";

        var syncConfig =
            @"{
              ""ChangeTrackingSyncOptions"": {
                ""UpsertVersionFunctionInfo"": {
                  ""FunctionName"": ""public.sync_info_upsert_by_table_name"",
                  ""RowVersionParameterName"": ""p_info""
                },
                ""ProviderVersionFunctionInfo"": {
                  ""FunctionName"": ""public.sync_info_get_last_for_table"",
                  ""TableParameterName"": ""p_table_name""
                }
              }
            }";

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(syncConfig)))
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(pgConfig)))
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(mssqlConfig)))
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Mock.Of<IHostApplicationLifetime>());
        services.AddSingleton<IConfiguration>(configuration);
        services.AddAutoMapper(
            cfg =>
            {
                cfg.AddProfile<TestMappingProfile>();
            });
        services.ConfigureCompositionRoot<DestinationEntity>(configuration);
        services.AddChangeTrackingTableSyncer(configuration.GetSection(nameof(ChangeTrackingSyncOptions)));

        var sp = services.BuildServiceProvider();
        return sp;
    }
}
