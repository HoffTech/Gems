// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Metrics.Contracts;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Metrics;

public enum ChangeTrackingSyncMetrics
{
    [Metric(
        Description = "Full load: load batch from source time (ms). Histogram.",
        Name = "sync_tables_full_load_batch_time_histogram",
        LabelNames = new[] { "sync_name" })]
    FullLoadSourceLoadTimeHistogram,

    [Metric(
        Description = "Full load: count of rows in batch. Histogram.",
        Name = "sync_tables_full_load_batch_rows_histogram",
        LabelNames = new[] { "sync_name" })]
    FullLoadDataCountHistogram,

    [Metric(
        Description = "Full load: count of transactions in batch. Histogram.",
        Name = "sync_tables_full_load_batch_transactions_histogram",
        LabelNames = new[] { "sync_name" })]
    FullLoadTransactionCountHistogram,

    [Metric(
        Description = "Full load: save batch time (ms). Histogram.",
        Name = "sync_tables_full_load_save_batch_time_histogram",
        LabelNames = new[] { "sync_name" })]
    FullLoadSaveTimeHistogram,

    [Metric(
        Description = "Changes load: load batch from source time (ms). Histogram.",
        Name = "sync_tables_changes_load_batch_time_histogram",
        LabelNames = new[] { "sync_name" })]
    ChangesSourceLoadTimeHistogram,

    [Metric(
        Description = "Changes load: count of rows in batch. Histogram.",
        Name = "sync_tables_changes_load_batch_rows_histogram",
        LabelNames = new[] { "sync_name" })]
    ChangesDataCountHistogram,

    [Metric(
        Description = "Changes load: count of transactions in batch. Histogram.",
        Name = "sync_tables_changes_load_batch_transactions_histogram",
        LabelNames = new[] { "sync_name" })]
    ChangesTransactionCountHistogram,

    [Metric(
        Description = "Changes load: save batch time (ms). Histogram.",
        Name = "sync_tables_changes_load_save_batch_time_histogram",
        LabelNames = new[] { "sync_name" })]
    ChangesSaveTimeHistogram,

    [Metric(
        Description = "Transform batch time (ms). Histogram.",
        Name = "sync_tables_transform_batch_time_histogram",
        LabelNames = new[] { "sync_name" })]
    BatchTransformTimeHistogram,

    [Metric(
        Description = "Changes load: Total count of inserts. Counter.",
        Name = "sync_tables_changes_insert_counter",
        LabelNames = new[] { "sync_name" })]
    ChangesInsertCountCounter,

    [Metric(
        Description = "Changes load: Total count of updates. Counter.",
        Name = "sync_tables_changes_update_counter",
        LabelNames = new[] { "sync_name" })]
    ChangesUpdateCountCounter,

    [Metric(
        Description = "Changes load: Total count of deletes. Counter.",
        Name = "sync_tables_changes_delete_counter",
        LabelNames = new[] { "sync_name" })]
    ChangesDeleteCountCounter,

    [Metric(
        Description = "Full load: Total count of inserts. Counter.",
        Name = "sync_tables_full_insert_counter",
        LabelNames = new[] { "sync_name" })]
    FullloadInsertCountCounter,
}
