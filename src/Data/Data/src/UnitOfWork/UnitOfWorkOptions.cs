// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
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

        private static readonly ConcurrentDictionary<string, Assembly> ScannedAssemblies = new ConcurrentDictionary<string, Assembly>();

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

        public string Key { get; set; }

        public SshClientOptions SshClientOptions { get; set; }

        internal Func<IConnectionStringProvider, bool, TimeMetricProvider, ILogger<IUnitOfWork>, CancellationToken, IUnitOfWork> Factory { get; set; }

        internal Action<Assembly> RegisterMappersInternal { get; set; }

        internal Action<Type> RegisterMapperInternal { get; set; }

        internal IConfiguration Configuration { get; set; }

        public void RegisterMappersFromAssemblyContaining<T>()
        {
            if (this.SuspendRegisterMappersFromAssemblyContaining)
            {
                return;
            }

            var targetAssembly = typeof(T).Assembly;
            if (ScannedAssemblies.TryGetValue(this.Key, out var assembly) && assembly == targetAssembly)
            {
                return;
            }

            if (ScannedAssemblies.TryAdd(this.Key, targetAssembly))
            {
                this.RegisterMappersInternal?.Invoke(targetAssembly);
            }
        }

        public void RegisterMapper<T>()
        {
            this.RegisterMapperInternal?.Invoke(typeof(T));
        }
    }
}
