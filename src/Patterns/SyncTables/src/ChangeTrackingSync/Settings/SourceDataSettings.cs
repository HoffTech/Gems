// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Settings;

public class SourceDataSettings
{
    public string DbKey { get; init; }

    public string TableName { get; init; }

    public string PrimaryKeyName { get; set; }

    public string ChangesQuery { get; init; }

    public string FullReloadQuery { get; init; }

    public SyncErrorAction OnDestinationVersionOutdated { get; init; }

    public SyncErrorAction OnRestoreFromBackupDetected { get; init; }

    public int GetCommandTimeout { get; init; } = 30;

    public int BatchSize { get; init; } = 100_000;
}
