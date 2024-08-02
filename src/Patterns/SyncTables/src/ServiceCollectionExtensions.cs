// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Patterns.SyncTables.MergeProcessor;
using Gems.Patterns.SyncTables.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Patterns.SyncTables
{
    public static class ServiceCollectionExtensions
    {
        [Obsolete("Если в используете для CT, то в 7.0 нужно будет использовать версию AddChangeTrackingTableSyncer")]
        public static void AddTableSyncer(this IServiceCollection services, IConfigurationSection section = null)
        {
            if (section is not null)
            {
                services.Configure<ChangeTrackingSyncOptions>(section);

                services.AddSingleton<RowVersionProvider>();
                services.AddSingleton<RowVersionUpdater>();

                services.AddSingleton<ChangeTrackingMergeProcessorFactory>();
            }

            services.AddSingleton<EntitiesUpdater>();
            services.AddSingleton<ExternalEntitiesProvider>();
            services.AddSingleton<MergeProcessorFactory>();
        }
    }
}
