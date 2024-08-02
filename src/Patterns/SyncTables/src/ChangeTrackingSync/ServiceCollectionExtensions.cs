// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Repository;
using Gems.Patterns.SyncTables.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using Prometheus;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync;

public static class ServiceCollectionExtensions
{
    public static void AddChangeTrackingTableSyncer(this IServiceCollection services, IConfigurationSection section)
    {
        services.Configure<ChangeTrackingSyncOptions>(section);

        services.AddSingleton<DestinationSyncedInfoProvider>();
        services.AddSingleton<SourceDbChangeTrackingInfoProvider>();
        services.AddSingleton<DestinationSyncedInfoUpdater>();
        services.AddSingleton<SourceEntitiesProvider>();
        services.AddSingleton<DestinationEntitiesUpdater>();
        services.AddTransient(typeof(IChangeTrackingSyncTableProcessor<,,>), typeof(ChangeTrackingSyncTableProcessor<,,>));

        ConfigureMetricsHistograms();

        // TODO remove when composite will have multiple scan target
        NpgsqlConnection.GlobalTypeMapper.MapComposite(typeof(SyncedInfo), "t_change_tracking_info");
    }

    private static void ConfigureMetricsHistograms()
    {
        Prometheus.Metrics.CreateHistogram(
            "sync_tables_changes_load_batch_time_histogram",
            "Changes load: load batch from source time (ms). Histogram.",
            new HistogramConfiguration
            {
                Buckets = new[] { 100.0, 500, 1000, 2000, 5000, 10000, 20000, 60000, 120000 },
                LabelNames = new[] { "sync_name" },
            });

        Prometheus.Metrics.CreateHistogram(
            "sync_tables_changes_load_batch_rows_histogram",
            "Changes load: count of rows in batch. Histogram.",
            new HistogramConfiguration
            {
                Buckets = new[] { 10.0, 25, 50, 100, 500, 1000, 5000, 10000, 25000, 50000 },
                LabelNames = new[] { "sync_name" },
            });

        Prometheus.Metrics.CreateHistogram(
            "sync_tables_changes_load_batch_transactions_histogram",
            "Changes load: count of transactions in batch. Histogram.",
            new HistogramConfiguration
            {
                Buckets = new[] { 1.0, 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000 },
                LabelNames = new[] { "sync_name" },
            });

        Prometheus.Metrics.CreateHistogram(
            "sync_tables_changes_load_save_batch_time_histogram",
            "Changes load: save batch time (ms). Histogram.",
            new HistogramConfiguration
            {
                Buckets = new[] { 100.0, 500, 1000, 2000, 5000, 10000, 20000, 60000, 120000 },
                LabelNames = new[] { "sync_name" },
            });

        Prometheus.Metrics.CreateHistogram(
            "sync_tables_full_load_batch_time_histogram",
            "Full load: load batch from source time (ms). Histogram.",
            new HistogramConfiguration
            {
                Buckets = new[] { 100.0, 500, 1000, 2000, 5000, 10000, 20000, 60000, 120000 },
                LabelNames = new[] { "sync_name" },
            });

        Prometheus.Metrics.CreateHistogram(
            "sync_tables_full_load_batch_rows_histogram",
            "Full load: count of rows in batch. Histogram.",
            new HistogramConfiguration
            {
                Buckets = new[] { 10.0, 25, 50, 100, 500, 1000, 5000, 10000, 25000, 50000 },
                LabelNames = new[] { "sync_name" },
            });

        Prometheus.Metrics.CreateHistogram(
            "sync_tables_full_load_batch_transactions_histogram",
            "Full load: count of transactions in batch. Histogram.",
            new HistogramConfiguration
            {
                Buckets = new[] { 1.0, 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000 },
                LabelNames = new[] { "sync_name" },
            });

        Prometheus.Metrics.CreateHistogram(
            "sync_tables_full_load_save_batch_time_histogram",
            "Full load: save batch time (ms). Histogram.",
            new HistogramConfiguration
            {
                Buckets = new[] { 100.0, 500, 1000, 2000, 5000, 10000, 20000, 60000, 120000 },
                LabelNames = new[] { "sync_name" },
            });

        Prometheus.Metrics.CreateHistogram(
            "sync_tables_transform_batch_time_histogram",
            "Transform batch time (ms). Histogram.",
            new HistogramConfiguration
            {
                Buckets = new[] { 1.0, 5, 10, 50, 100, 250, 500, 1000, 2000, 5000, 10000 },
                LabelNames = new[] { "sync_name" },
            });
    }
}
