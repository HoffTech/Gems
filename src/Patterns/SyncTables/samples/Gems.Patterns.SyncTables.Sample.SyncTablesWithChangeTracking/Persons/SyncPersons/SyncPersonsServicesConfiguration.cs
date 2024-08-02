// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc;
using Gems.Patterns.SyncTables.ChangeTrackingSync;
using Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons.Options;

namespace Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons;

public class SyncPersonsServicesConfiguration : IServicesConfiguration
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SyncPersonsInfoOptions>(configuration.GetSection(SyncPersonsInfoOptions.SectionName));
        services.AddChangeTrackingTableSyncer(configuration.GetSection(ImportPersonsFromDaxOptions.SectionName));
    }
}
