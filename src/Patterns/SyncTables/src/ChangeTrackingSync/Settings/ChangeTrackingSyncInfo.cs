// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Patterns.SyncTables.ChangeTrackingSync.Exceptions;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Settings;

public class ChangeTrackingSyncInfo
{
    public ChangeTrackingSyncInfo(
        SourceDataSettings sourceSettings,
        DestinationSettings destinationSettings,
        bool needConvertDateTimeToUtc)
    {
        if (sourceSettings.OnRestoreFromBackupDetected == SyncErrorAction.FullReload
            && (string.IsNullOrEmpty(sourceSettings.FullReloadQuery)
                || string.IsNullOrEmpty(destinationSettings.ClearFunctionName)))
        {
            throw new SyncBadConfigurationException(
                $"Should configure {nameof(sourceSettings.FullReloadQuery)} and {nameof(destinationSettings.ClearFunctionName)} " +
                $"when select {nameof(sourceSettings.OnRestoreFromBackupDetected)} mode {nameof(SyncErrorAction.FullReload)}");
        }

        if (sourceSettings.OnDestinationVersionOutdated == SyncErrorAction.FullReload
            && (string.IsNullOrEmpty(sourceSettings.FullReloadQuery)
                || string.IsNullOrEmpty(destinationSettings.ClearFunctionName)))
        {
            throw new SyncBadConfigurationException(
                $"Should configure {nameof(sourceSettings.FullReloadQuery)} and {nameof(destinationSettings.ClearFunctionName)} " +
                $"when select {nameof(sourceSettings.OnRestoreFromBackupDetected)} mode {nameof(SyncErrorAction.FullReload)}");
        }

        this.SourceSettings = sourceSettings;
        this.DestinationSettings = destinationSettings;
        this.NeedConvertDateTimeToUtc = needConvertDateTimeToUtc;
    }

    public SourceDataSettings SourceSettings { get; }

    public DestinationSettings DestinationSettings { get; }

    public bool NeedConvertDateTimeToUtc { get; }
}
