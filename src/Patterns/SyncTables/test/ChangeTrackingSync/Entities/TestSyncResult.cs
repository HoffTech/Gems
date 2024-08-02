// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.Entities;

public class TestSyncResult : IDestinationEntitiesHolder<RealDestinationEntity>
{
    public IEnumerable<RealDestinationEntity> Entities { get; set; }
}
