// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Patterns.SyncTables.EntitiesViews
{
    public class ExternalChangeTrackingEntity : ExternalEntity
    {
        public long RowVersion { get; set; }

        public string OperationType { get; set; }
    }
}
