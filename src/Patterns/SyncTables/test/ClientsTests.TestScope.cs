// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;
using Gems.Linq;
using Gems.Patterns.SyncTables.Options;
using Gems.Patterns.SyncTables.Tests.Infrastructure;
using Gems.Patterns.SyncTables.Tests.Infrastructure.Clients;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Moq;

namespace Gems.Patterns.SyncTables.Tests
{
    public partial class ClientsTests
    {
        private class TestScope : IAsyncDisposable
        {
            private IServiceProvider serviceProvider;
            private Mock<IUnitOfWork> unitOfWorkWork;

            public MergeResult MergeResult { get; set; }

            public List<RealExternalEntity> ExternalEntities { get; set; }

            public List<RealExternalChangeTrackingEntity> ExternalChangeTrackingEntities { get; set; }

            public string SourceDbKey { get; set; }

            public string TargetDbKey { get; set; }

            public void BuildServiceProvider(Action<ServiceCollection> configureServices = null)
            {
                var configuration = BuildConfiguration();

                var services = new ServiceCollection();

                services.AddLogging();
                services.AddAutoMapper(cfg => { cfg.AddProfile<ClientsMappingProfile>(); });

                services.AddSingleton<ChangeTrackingMergeClient>();
                services.AddSingleton<MergeClient>();
                services.AddTableSyncer(configuration.GetSection(nameof(ChangeTrackingSyncOptions)));

                configureServices?.Invoke(services);

                this.MockUnitOfWork(services);
                this.MockGetLastRowVersionForTableReturnsZeroVersion();

                if (!this.ExternalEntities.IsNullOrEmpty())
                {
                    this.MockGetExternalEntitiesByQueryReturnsExternalEntities(this.ExternalEntities);
                }

                if (!this.ExternalChangeTrackingEntities.IsNullOrEmpty())
                {
                    this.MockGetExternalChangeTrackingEntitiesByQueryWithVersionReturnsExternalEntities(this.ExternalChangeTrackingEntities);
                }

                this.MockMergeEntitiesReturnsMergeResult(this.MergeResult);

                this.serviceProvider = services.BuildServiceProvider();
            }

            public TService GetRequiredService<TService>() where TService : notnull
            {
                return this.serviceProvider.GetRequiredService<TService>();
            }

            public ValueTask DisposeAsync()
            {
                return this.serviceProvider is IAsyncDisposable disposable
                    ? disposable.DisposeAsync()
                    : ValueTask.CompletedTask;
            }

            public void VerifyUpsertTableVersionCallTimesExactly(int callCount)
            {
                var options = this.serviceProvider.GetRequiredService<IOptions<ChangeTrackingSyncOptions>>();

                this.unitOfWorkWork
                    .Verify(
                        x => x.CallStoredProcedureAsync(
                            It.Is<string>(s => s.Equals(options.Value.UpsertVersionFunctionInfo.FunctionName)),
                            It.Is<Dictionary<string, object>>(
                                y =>
                                    y.ContainsKey(options.Value.UpsertVersionFunctionInfo.TableParameterName) &&
                                    y.ContainsKey(options.Value.UpsertVersionFunctionInfo.RowVersionParameterName)),
                            It.IsAny<Enum>()),
                        Times.Exactly(callCount));
            }

            private static IConfiguration BuildConfiguration()
            {
                var configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ChangeTrackingSyncOptions:SourceDbKeyProvider"] = "Axapta",
                    ["ChangeTrackingSyncOptions:UpsertVersionFunctionInfo:FunctionName"] = "FunctionName",
                    ["ChangeTrackingSyncOptions:UpsertVersionFunctionInfo:TableParameterName"] = "TableParameterName",
                    ["ChangeTrackingSyncOptions:UpsertVersionFunctionInfo:RowVersionParameterName"] = "RowVersionParameterName",
                    ["ChangeTrackingSyncOptions:ProviderVersionFunctionInfo:FunctionName"] = "FunctionName",
                    ["ChangeTrackingSyncOptions:ProviderVersionFunctionInfo:TableParameterName"] = "TableParameterName"
                });
                return configurationBuilder.Build();
            }

            private void MockUnitOfWork(IServiceCollection services)
            {
                this.unitOfWorkWork = new Mock<IUnitOfWork>();

                var unitOfWorkProviderMock = new Mock<IUnitOfWorkProvider>();
                unitOfWorkProviderMock
                    .Setup(x => x.GetUnitOfWork(It.IsAny<CancellationToken>()))
                    .Returns(this.unitOfWorkWork.Object);

                unitOfWorkProviderMock
                    .Setup(x => x.GetUnitOfWork(
                        this.SourceDbKey,
                        It.IsAny<CancellationToken>()))
                    .Returns(this.unitOfWorkWork.Object);

                unitOfWorkProviderMock
                    .Setup(x => x.GetUnitOfWork(
                        this.TargetDbKey,
                        It.IsAny<CancellationToken>()))
                    .Returns(this.unitOfWorkWork.Object);

                services.AddSingleton(unitOfWorkProviderMock.Object);
            }

            private void MockGetLastRowVersionForTableReturnsZeroVersion()
            {
                this.unitOfWorkWork
                    .Setup(x => x.CallScalarFunctionAsync<long>(
                        It.IsAny<string>(),
                        It.IsAny<Dictionary<string, object>>(),
                        It.IsAny<Enum>()))
                    .ReturnsAsync(default(long));
            }

            private void MockGetExternalChangeTrackingEntitiesByQueryWithVersionReturnsExternalEntities(
                List<RealExternalChangeTrackingEntity> externalEntities)
            {
                this.unitOfWorkWork
                    .Setup(x => x.QueryAsync<RealExternalChangeTrackingEntity>(
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.Is<Dictionary<string, object>>(y => y.ContainsKey("@version")),
                        It.IsAny<Enum>()))
                    .ReturnsAsync(externalEntities);
            }

            private void MockGetExternalEntitiesByQueryReturnsExternalEntities(List<RealExternalEntity> externalEntities)
            {
                this.unitOfWorkWork
                    .Setup(x => x.QueryAsync<RealExternalEntity>(
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<Enum>()))
                    .ReturnsAsync(externalEntities);
            }

            private void MockMergeEntitiesReturnsMergeResult(MergeResult mergeResult)
            {
                this.unitOfWorkWork
                    .Setup(x => x.CallTableFunctionFirstAsync<MergeResult>(
                        It.IsAny<string>(),
                        It.IsAny<Dictionary<string, object>>(),
                        It.IsAny<Enum>()))
                    .ReturnsAsync(mergeResult);
            }
        }
    }
}
