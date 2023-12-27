// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

using Gems.Metrics;
using Gems.Metrics.Contracts;
using Gems.Metrics.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Gems.Data.Tests")]
[assembly: InternalsVisibleTo("Gems.Data.Npgsql")]
[assembly: InternalsVisibleTo("Gems.Data.MySql")]
[assembly: InternalsVisibleTo("Gems.Data.SqlServer")]

namespace Gems.Data.UnitOfWork
{
    public class UnitOfWorkOptions
    {
        public const string DefaultKey = "default";

        private static readonly ConcurrentDictionary<string, ConcurrentBag<Assembly>> ScannedAssemblies = new ConcurrentDictionary<string, ConcurrentBag<Assembly>>();

        public string ConnectionString { get; set; }

        /// <summary>
        /// Игнорировать создание транзакций (флаг needTransaction).
        /// </summary>
        public bool SuspendTransaction { get; set; }

        public Enum DbQueryMetricType
        {
            set => this.DbQueryMetricInfo = MetricNameHelper.GetMetricInfo(value);
        }

        public MetricInfo? DbQueryMetricInfo { get; set; }

        public bool SuspendRegisterMappersFromAssemblyContaining { get; set; }

        public string Key { get; set; } = DefaultKey;

        public SshClientOptions SshClientOptions { get; set; }

        internal Func<IConnectionStringProvider, bool, TimeMetricProvider, ILogger<IUnitOfWork>, SshClientOptions, CancellationToken, IUnitOfWork> Factory { get; set; }

        internal Action<Assembly> RegisterMappersInternal { get; set; }

        internal Action<Type> RegisterMapperInternal { get; set; }

        internal IConfiguration Configuration { get; set; }

        public void RegisterMappersFromAssemblyContaining<T>()
        {
            if (this.SuspendRegisterMappersFromAssemblyContaining)
            {
                return;
            }

            var assemblies = ScannedAssemblies.GetOrAdd(this.Key, new ConcurrentBag<Assembly>());

            var targetAssembly = typeof(T).Assembly;
            if (assemblies.Any(x => x == targetAssembly))
            {
                return;
            }

            this.RegisterMappersInternal?.Invoke(targetAssembly);
            assemblies.Add(targetAssembly);
        }

        public void RegisterMapper<T>()
        {
            this.RegisterMapperInternal?.Invoke(typeof(T));
        }
    }
}
