// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoMapper;

using Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.Entities;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync;

public class ClientsMappingProfile : Profile
{
    public ClientsMappingProfile()
    {
        this.CreateMap<RealSourceChangeTrackingEntity, RealDestinationEntity>();
    }
}
