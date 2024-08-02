// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync;

public class SessionContext<TMergeResult>
{
    public SyncedInfo DestinationInfo { get; set; }

    public bool? IsFullReloadSession { get; set; }

    public bool NeedToSyncDataAgain { get; set; } = true;

    public long Offset { get; set; }

    public List<(long version, long offset, TMergeResult mergeResult)> Results { get; set; } = new List<(long version, long offset, TMergeResult mergeResult)>();

    public bool SessionDataCleared { get; set; }

    public ChangeTrackingInfo SessionSourceChangeTrackingVersionInfo { get; set; }

    public ChangeTrackingInfo SourceChangeTrackingVersionInfo { get; set; }
}
