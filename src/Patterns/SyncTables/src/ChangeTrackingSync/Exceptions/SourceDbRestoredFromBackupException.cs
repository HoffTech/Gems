// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Exceptions;

public class SourceDbRestoredFromBackupException : Exception
{
    public SourceDbRestoredFromBackupException(string message) : base(message)
    {
    }
}
