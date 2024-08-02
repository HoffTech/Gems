// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Gems.Data.Npgsql;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

using NpgsqlTypes;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.IntegrationTests.Entities;

[PgType("t_destination_merge_result")]
public class DestinationSyncMergeResult : IDestinationEntitiesHolder<DestinationEntity>
{
    [PgName("deleted_count")]
    public int DeletedCount { get; set; }

    [PgName("inserted_count")]
    public int InsertedCount { get; set; }

    [PgName("updated_count")]
    public int UpdatedCount { get; set; }

    public IEnumerable<DestinationEntity> Entities { get; set; }
}
