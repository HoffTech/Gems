// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Npgsql;

using NpgsqlTypes;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync.IntegrationTests.Entities;

[PgType("public.t_destination_entity")]
public class DestinationEntity
{
    [PgName("ct_version")]
    public long ChangeTrackingVersion { get; set; }

    [PgName("operation_type")]
    public string OperationType { get; set; }

    [PgName("rec_id")]
    public long RecId { get; set; }

    [PgName("item_id")]
    public string ItemId { get; set; }

    [PgName("text_data")]
    public string TextData { get; set; }

    [PgName("numeric_data")]
    public decimal NumericData { get; set; }
}
