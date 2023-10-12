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

namespace Gems.Data.SqlServer
{
    /// <summary>
    /// Class with middleware extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void AddMsSqlUnitOfWork(this IServiceCollection services, Action<UnitOfWorkOptions> configureOptions = null)
        {
            AddMsSqlUnitOfWork(services, UnitOfWorkOptions.DefaultKey, null, configureOptions);
        }

        public static void AddMsSqlUnitOfWork(this IServiceCollection services, IConfiguration configuration, Action<UnitOfWorkOptions> configureOptions = null)
        {
            var msSqlUnitOfWorkOptions = configuration.GetSection(MsSqlUnitOfWorkOptionsList.Name).Get<MsSqlUnitOfWorkOptionsList>();
            var options = msSqlUnitOfWorkOptions.FirstOrDefault(x => x.Key == UnitOfWorkOptions.DefaultKey)?.Options;
            SetConnectionString(configuration, options);
            AddMsSqlUnitOfWork(services, UnitOfWorkOptions.DefaultKey, options, configureOptions);
        }

        public static void AddMsSqlUnitOfWork(this IServiceCollection services, string key, Action<UnitOfWorkOptions> configureOptions = null)
        {
            AddMsSqlUnitOfWork(services, key, null, configureOptions);
        }

        public static void AddMsSqlUnitOfWork(this IServiceCollection services, IConfiguration configuration, string key, Action<UnitOfWorkOptions> configureOptions = null)
        {
            var msSqlUnitOfWorkOptions = configuration.GetSection(MsSqlUnitOfWorkOptionsList.Name).Get<MsSqlUnitOfWorkOptionsList>();
            var options = msSqlUnitOfWorkOptions.FirstOrDefault(x => x.Key == key)?.Options;
            SetConnectionString(configuration, options);
            AddMsSqlUnitOfWork(services, key, options, configureOptions);
        }

        public static void AddMsSqlUnitOfWork(this IServiceCollection services, string key, UnitOfWorkOptions options, Action<UnitOfWorkOptions> configureOptions = null)
        {
            options ??= new UnitOfWorkOptions();
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
            SshClientOptions sshClientOptions,
            CancellationToken cancellationToken)
        {
            return new SqlUnitOfWork(connectionStringProvider, needTransaction, timeMetricProvider, cancellationToken);
        }
    }
}
