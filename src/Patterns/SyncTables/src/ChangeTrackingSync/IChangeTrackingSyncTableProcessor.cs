// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Settings;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync;

public interface IChangeTrackingSyncTableProcessor<TSourceEntity, TDestinationEntity, TMergeResult>
    where TSourceEntity : class, ISourceChangeTrackingEntity
    where TDestinationEntity : class
    where TMergeResult : class, new()
{
    Task<SyncTableResult<TMergeResult>> Sync(
        ChangeTrackingSyncInfo syncInfo,
        CancellationToken cancellationToken);
}
