// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Patterns.SyncTables.MergeProcessor;

public interface ITargetEntitiesHolder
{
    IEnumerable<object> Entities { get; set; }
}
