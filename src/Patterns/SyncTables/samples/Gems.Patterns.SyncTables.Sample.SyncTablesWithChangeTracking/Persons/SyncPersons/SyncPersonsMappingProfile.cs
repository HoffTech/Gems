// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoMapper;

using Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons.EntitiesViews;

namespace Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons;

public class SyncPersonsMappingProfile : Profile
{
    public SyncPersonsMappingProfile()
    {
        this.CreateMap<ExternalPerson, Person>();
    }
}
