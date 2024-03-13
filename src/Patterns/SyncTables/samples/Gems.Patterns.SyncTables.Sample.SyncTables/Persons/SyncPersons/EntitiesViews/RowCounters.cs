// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Npgsql;
using Gems.Patterns.SyncTables.MergeProcessor;

using NpgsqlTypes;

namespace Gems.Patterns.SyncTables.Sample.SyncTables.Persons.SyncPersons.EntitiesViews;

[PgType("public.row_counters_type")]
public class RowCounters : ITargetEntitiesHolder
{
    [PgName("table_name")]
    public string TableName { get; set; }

    [PgName("inserted_count")]
    public int InsertedRowsCount { get; set; }

    [PgName("updated_count")]
    public int UpdatedRowsCount { get; set; }

    [PgName("deleted_count")]
    public int DeletedRowsCount { get; set; }

    [PgName("inserted_ids")]
    public string[] InsertedIds { get; set; }

    public IEnumerable<object> Entities { get; set; }
}
