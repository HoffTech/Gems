// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

public interface ISourceChangeTrackingEntity
{
    public long ChangeTrackingVersion { get; set; }

    public string OperationType { get; set; }
}
