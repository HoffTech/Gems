// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc;

namespace Gems.Patterns.SyncTables.Sample.SyncTables.Persons.SyncPersons;

public class SyncPersonsServicesConfiguration : IServicesConfiguration
{
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        services.AddTableSyncer();
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
