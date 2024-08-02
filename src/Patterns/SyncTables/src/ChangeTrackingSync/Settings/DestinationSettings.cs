// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Settings;

public class DestinationSettings
{
    public string DbKey { get; init; }

    public string TableName { get; init; }

    public string MergeFunctionName { get; init; }

    public string ClearFunctionName { get; init; }

    public string MergeParameterName { get; init; }

    public bool EnableFullChangesLog { get; set; }

    public int MergeCommandTimeout { get; set; } = 30;
}
