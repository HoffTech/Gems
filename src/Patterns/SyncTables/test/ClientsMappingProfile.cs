// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoMapper;

using Gems.Patterns.SyncTables.Tests.Infrastructure;

namespace Gems.Patterns.SyncTables.Tests
{
    public class ClientsMappingProfile : Profile
    {
        public ClientsMappingProfile()
        {
            this.CreateMap<RealExternalEntity, RealTargetEntity>();
            this.CreateMap<RealExternalChangeTrackingEntity, RealTargetEntity>();
        }
    }
}
