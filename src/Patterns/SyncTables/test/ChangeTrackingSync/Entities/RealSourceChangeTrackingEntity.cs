// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.Entities;
#nullable disable

public class RealSourceChangeTrackingEntity : ISourceChangeTrackingEntity
{
    public string Name { get; set; }

    public string Age { get; set; }

    public long ChangeTrackingVersion { get; set; }

    public string OperationType { get; set; }
}
