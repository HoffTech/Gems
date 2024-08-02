// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoMapper;

using Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.IntegrationTests.Entities;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.IntegrationTests;

public class TestMappingProfile : Profile
{
    public TestMappingProfile()
    {
        this.CreateMap<SourceEntity, DestinationEntity>();
    }
}
