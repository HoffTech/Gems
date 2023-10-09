// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MySqlConnector;

using NUnit.Framework;

namespace Gems.Data.MySql.Tests;

[Parallelizable(ParallelScope.All)]
[Ignore("Ignore all tests")]
public class MySqlDapperHelperTests
{
    [OneTimeSetUp]
    public void Init()
    {
        BuildServiceProvider((services, configuration) => services.AddMySqlUnitOfWork(configuration));
    }

    [Test]
    public async Task QueryAsync_ShouldInvokeWithoutParameters()
    {
        var serviceProvider = BuildServiceProvider();
        var connectionString = serviceProvider.GetService<IConfiguration>().GetConnectionString("MySql");
        var user = await MySqlDapperHelper.QueryFirstOrDefaultAsync<User>(connectionString, "SELECT Id, Name FROM Users LIMIT 1;", CancellationToken.None);
        Assert.NotNull(user);
        Assert.AreEqual(1, user.Id);
        Assert.AreEqual("User1", user.Name);
    }

    [Test]
    public async Task QueryAsync_ShouldInvokeWithParameters()
    {
        var serviceProvider = BuildServiceProvider();
        var connectionString = serviceProvider.GetService<IConfiguration>().GetConnectionString("MySql");
        var user = await MySqlDapperHelper.QueryFirstOrDefaultAsync<User>(
            connectionString,
            "SELECT Id, Name FROM Users WHERE Id = @Id;",
            new Dictionary<string, object>
            {
                ["Id"] = 1
            },
            CancellationToken.None);
        Assert.NotNull(user);
        Assert.AreEqual(1, user.Id);
        Assert.AreEqual("User1", user.Name);
    }

    [Test]
    public async Task QueryAsync_ShouldInvokeWithDynamicParameters()
    {
        var serviceProvider = BuildServiceProvider();
        var connectionString = serviceProvider.GetService<IConfiguration>().GetConnectionString("MySql");
        var parameters = new DynamicParameters();
        parameters.Add("Id", 1);
        var user = await MySqlDapperHelper.QueryFirstOrDefaultAsync<User>(
            connectionString,
            "SELECT Id, Name FROM Users WHERE Id = @Id;",
            parameters,
            CancellationToken.None);
        Assert.NotNull(user);
        Assert.AreEqual(1, user.Id);
        Assert.AreEqual("User1", user.Name);
    }

    [Test]
    public async Task QueryAsync_ShouldInsertUser()
    {
        var serviceProvider = BuildServiceProvider();
        var connectionString = serviceProvider.GetService<IConfiguration>().GetConnectionString("MySql");
        var uniqId = int.MaxValue - 1;
        await CreateUser(connectionString, uniqId);
        var user = await GetUser(connectionString, uniqId);

        Assert.NotNull(user);
        Assert.AreEqual(uniqId, user.Id);
        Assert.AreEqual($"User{uniqId}", user.Name);

        await DeleteUser(connectionString, uniqId);
    }

    [Test]
    public async Task QueryAsync_ShouldInsertUserInsideTransaction()
    {
        var serviceProvider = BuildServiceProvider();
        var connectionString = serviceProvider.GetService<IConfiguration>().GetConnectionString("MySql");
        var uniqId = int.MaxValue;
        await CreateUser(connectionString, uniqId, true);
        var user = await GetUser(connectionString, uniqId);

        Assert.NotNull(user);
        Assert.AreEqual(uniqId, user.Id);
        Assert.AreEqual($"User{uniqId}", user.Name);

        await DeleteUser(connectionString, uniqId);
    }

    [Test]
    public async Task QueryAsync_ShouldInsertUserWithTimeout()
    {
        var serviceProvider = BuildServiceProvider();
        var connectionString = serviceProvider.GetService<IConfiguration>().GetConnectionString("MySql");
        var uniqId = int.MaxValue;
        await CreateUser(connectionString, uniqId, true, 1);
        var user = await GetUser(connectionString, uniqId);

        Assert.NotNull(user);
        Assert.AreEqual(uniqId, user.Id);
        Assert.AreEqual($"User{uniqId}", user.Name);

        await DeleteUser(connectionString, uniqId);
    }

    private static async Task DeleteUser(string connectionString, int uniqId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Id", uniqId);
        await MySqlDapperHelper.QueryFirstOrDefaultAsync<User>(
            connectionString,
            "DELETE FROM Users WHERE Id = @Id;",
            parameters,
            CancellationToken.None);
    }

    private static async Task<User> GetUser(string connectionString, int uniqId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Id", uniqId);
        var user = await MySqlDapperHelper.QueryFirstOrDefaultAsync<User>(
            connectionString,
            "SELECT Id, Name FROM Users WHERE Id = @Id;",
            parameters,
            CancellationToken.None);
        return user;
    }

    private static async Task CreateUser(string connectionString, int uniqId, bool needTransaction = false, int timeout = 0)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Id", uniqId);
        parameters.Add("Name", $"User{uniqId}");
        await using var connection = new MySqlConnection(connectionString);
        MySqlTransaction transaction = null;
        var cancellationToken = CancellationToken.None;
        if (needTransaction)
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        await MySqlDapperHelper.QueryFirstOrDefaultAsync<User>(
            connection,
            transaction,
            "INSERT INTO Users  (Id, Name) VALUES(@Id, @Name);",
            parameters,
            timeout,
            cancellationToken);
        if (needTransaction)
        {
            await transaction.CommitAsync(cancellationToken);
        }
    }

    private static IServiceProvider BuildServiceProvider(Action<ServiceCollection, IConfiguration> configureServices = null)
    {
        var configuration = BuildConfiguration();

        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        configureServices?.Invoke(services, configuration);
        return services.BuildServiceProvider();
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("Gems.Data.MySql.Tests.Settings.Local.json")
            .AddEnvironmentVariables()
            .Build();
    }
}
