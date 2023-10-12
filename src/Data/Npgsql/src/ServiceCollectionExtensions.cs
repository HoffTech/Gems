// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Threading;

using Gems.Data.UnitOfWork;
using Gems.Metrics.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gems.Data.Npgsql
{
    /// <summary>
    /// Class with middleware extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void AddPostgresqlUnitOfWork(this IServiceCollection services, Action<UnitOfWorkOptions> configureOptions = null)
        {
            AddPostgresqlUnitOfWork(services, UnitOfWorkOptions.DefaultKey, null, configureOptions);
        }

        public static void AddPostgresqlUnitOfWork(this IServiceCollection services, IConfiguration configuration, Action<UnitOfWorkOptions> configureOptions = null)
        {
            var postgresqlUnitOfWorkOptions = configuration.GetSection(PostgresqlUnitOfWorkOptionsList.Name).Get<PostgresqlUnitOfWorkOptionsList>();
            var options = postgresqlUnitOfWorkOptions.FirstOrDefault(x => x.Key == UnitOfWorkOptions.DefaultKey)?.Options;
            SetConnectionString(configuration, options);
            AddPostgresqlUnitOfWork(services, UnitOfWorkOptions.DefaultKey, options, configureOptions);
        }

        public static void AddPostgresqlUnitOfWork(this IServiceCollection services, string key, Action<UnitOfWorkOptions> configureOptions = null)
        {
            AddPostgresqlUnitOfWork(services, key, null, configureOptions);
        }

        public static void AddPostgresqlUnitOfWork(this IServiceCollection services, IConfiguration configuration, string key, Action<UnitOfWorkOptions> configureOptions = null)
        {
            var postgresqlUnitOfWorkOptions = configuration.GetSection(PostgresqlUnitOfWorkOptionsList.Name).Get<PostgresqlUnitOfWorkOptionsList>();
            var options = postgresqlUnitOfWorkOptions.FirstOrDefault(x => x.Key == key)?.Options;
            SetConnectionString(configuration, options);
            AddPostgresqlUnitOfWork(services, key, options, configureOptions);
        }

        private static void AddPostgresqlUnitOfWork(this IServiceCollection services, string key, UnitOfWorkOptions options, Action<UnitOfWorkOptions> configureOptions = null)
        {
            options ??= new UnitOfWorkOptions();
            options.RegisterMappersInternal = PgSqlMapper.RegisterMappers;
            options.RegisterMapperInternal = PgSqlMapper.RegisterMapper;
            options.Key = key;
            options.Factory = CreateUnitOfWork;
            configureOptions?.Invoke(options);
            services.AddSingleton(options);

            services.AddCommonUnitOfWorkServices();
        }

        private static void SetConnectionString(IConfiguration configuration, UnitOfWorkOptions options)
        {
            if (!(options?.ConnectionString?.StartsWith("${ConnectionStrings.") ?? false))
            {
                return;
            }

            options.ConnectionString = options.ConnectionString.Replace("${ConnectionStrings.", string.Empty);
            options.ConnectionString = options.ConnectionString.TrimEnd('}');
            options.ConnectionString = configuration.GetConnectionString(options.ConnectionString);
        }

        private static IUnitOfWork CreateUnitOfWork(
            IConnectionStringProvider connectionStringProvider,
            bool needTransaction,
            TimeMetricProvider timeMetricProvider,
            ILogger<IUnitOfWork> logger,
            CancellationToken cancellationToken)
        {
            return new NpgsqlUnitOfWork(connectionStringProvider, needTransaction, timeMetricProvider, cancellationToken);
        }
    }
}
