// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Patterns.SyncTables.ChangeTrackingSync;

public class SyncTableResult<TMergeResult>
{
    public SyncTableResult(bool isFullReloadSession, TMergeResult[] mergeResults)
    {
        this.IsFullReloadSession = isFullReloadSession;
        this.MergeResults = mergeResults;
    }

    public bool IsFullReloadSession { get; set; }

    public TMergeResult[] MergeResults { get; set; }
}
