// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.IntegrationTests.Entities;

public class SourceEntity : ISourceChangeTrackingEntity
{
    public long RecId { get; set; }

    public string ItemId { get; set; }

    public string TextData { get; set; }

    public decimal NumericData { get; set; }

    public long ChangeTrackingVersion { get; set; }

    public string OperationType { get; set; }
}
