// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

public interface IDestinationEntitiesHolder<T>
{
    IEnumerable<T> Entities { get; set; }
}
