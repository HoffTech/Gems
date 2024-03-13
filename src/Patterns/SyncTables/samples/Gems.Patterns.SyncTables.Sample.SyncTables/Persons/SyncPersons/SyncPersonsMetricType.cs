// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Metrics.Contracts;

namespace Gems.Patterns.SyncTables.Sample.SyncTables.Persons.SyncPersons;

public enum SyncPersonsMetricType
{
    [Metric(
        Name = "mssql_db_query_time",
        LabelNames = new[] { "query_name" },
        LabelValues = new[] { "get_external_person_entities" })]
    GetExternalPersonEntities
}
