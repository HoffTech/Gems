// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Context;
using Gems.Metrics.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gems.Data.UnitOfWork
{
    public class UnitOfWorkProvider : IUnitOfWorkProvider
    {
        /// <summary>
        /// Ключ, по которому хранится unitOfWorkMap в контексте.
        /// </summary>
        public const string UnitOfWorkMapName = "__UnitOfWorkMap__";

        /// <summary>
        /// Использовать контекст. Если true, то UnitOfWork будет хранится в контексте, иначе в провайдере UnitOfWork.
        /// </summary>
        public const string UseContextConfigPath = "UnitOfWorkProviderOptions:UseContext";

        private readonly IConfiguration configuration;
        private readonly ITimeMetricProviderFactory metricProviderFactory;
        private readonly IContextAccessor contextAccessor;
        private readonly ILogger<IUnitOfWork> logger;
        private readonly Dictionary<string, UnitOfWorkOptions> optionsMap;
        private readonly Dictionary<string, ConcurrentDictionary<CancellationToken, IUnitOfWork>> unitOfWorkMap;

        public UnitOfWorkProvider(
            IEnumerable<UnitOfWorkOptions> options,
            IConfiguration configuration,
            ITimeMetricProviderFactory metricProviderFactory,
            IContextAccessor contextAccessor,
            ILogger<IUnitOfWork> logger)
        {
            this.configuration = configuration;
            this.metricProviderFactory = metricProviderFactory;
            this.contextAccessor = contextAccessor;
            this.logger = logger;
            this.optionsMap = options.ToDictionary(x => x.Key);
            this.unitOfWorkMap = options.ToDictionary(x => x.Key, _ => new ConcurrentDictionary<CancellationToken, IUnitOfWork>());
        }

        public IUnitOfWork GetUnitOfWork(string key, bool needTransaction, CancellationToken cancellationToken)
        {
            if (!this.optionsMap.TryGetValue(key, out var options))
            {
                throw new NotImplementedException($"Options for unit of work '{key}' not registered.");
            }

            var context = this.contextAccessor.Context;
            var unitOfWorks = this.GetUnitOfWorksMapFromContextOrLocal(context)[key];
            var unitOfWork = unitOfWorks.GetOrAdd(cancellationToken, ct => this.CreateUnitOfWork(options, needTransaction, ct));
            this.logger.LogTrace($"UnitOfWork: {unitOfWork.GetHashCode()}");
            return unitOfWork;
        }

        public IUnitOfWork GetUnitOfWork(string key, CancellationToken cancellationToken)
        {
            return this.GetUnitOfWork(key, false, cancellationToken);
        }

        public IUnitOfWork GetUnitOfWork(bool needTransaction, CancellationToken cancellationToken)
        {
            return this.GetUnitOfWork(UnitOfWorkOptions.DefaultKey, needTransaction, cancellationToken);
        }

        public IUnitOfWork GetUnitOfWork(CancellationToken cancellationToken)
        {
            return this.GetUnitOfWork(UnitOfWorkOptions.DefaultKey, false, cancellationToken);
        }

        public List<IUnitOfWork> GetUnitOfWorks(bool needTransaction, CancellationToken cancellationToken)
        {
            var context = this.contextAccessor.Context;
            return this.GetUnitOfWorksMapFromContextOrLocal(context).Keys.Select(key => this.GetUnitOfWork(key, needTransaction, cancellationToken)).ToList();
        }

        public async Task RemoveUnitOfWorkAsync(string key, CancellationToken cancellationToken)
        {
            var context = this.contextAccessor.Context;
            if (this.GetUnitOfWorksMapFromContextOrLocal(context)[key].TryRemove(cancellationToken, out var unitOfWork))
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public Task RemoveUnitOfWorkAsync(CancellationToken cancellationToken)
        {
            return this.RemoveUnitOfWorkAsync(UnitOfWorkOptions.DefaultKey, cancellationToken);
        }

        public async Task RemoveUnitOfWorksAsync(CancellationToken cancellationToken)
        {
            var context = this.contextAccessor.Context;
            foreach (var key in this.GetUnitOfWorksMapFromContextOrLocal(context).Keys)
            {
                await this.RemoveUnitOfWorkAsync(key, cancellationToken);
            }
        }

        public bool CheckUnitOfWork(CancellationToken cancellationToken)
        {
            var context = this.contextAccessor.Context;
            return this.GetUnitOfWorksMapFromContextOrLocal(context).Values.Any(unitOfWorks => unitOfWorks.TryGetValue(cancellationToken, out _));
        }

        private Dictionary<string, ConcurrentDictionary<CancellationToken, IUnitOfWork>> GetUnitOfWorksMapFromContextOrLocal(IContext context)
        {
            this.logger.LogTrace($"Context: {context?.GetHashCode()}");
            if (context == null || !this.configuration.GetValue<bool>(UseContextConfigPath))
            {
                this.logger.LogTrace($"UnitOfWorkMap: {this.unitOfWorkMap.GetHashCode()}");
                return this.unitOfWorkMap;
            }

            if (context.Items.TryGetValue(UnitOfWorkMapName, out var unitOfWorkMapAsObject) &&
                unitOfWorkMapAsObject is Dictionary<string, ConcurrentDictionary<CancellationToken, IUnitOfWork>> unitOfWorkMapFromContext)
            {
                this.logger.LogTrace($"UnitOfWorkMap: {unitOfWorkMapFromContext.GetHashCode()}");
                return unitOfWorkMapFromContext;
            }

            throw new InvalidOperationException("Something went wrong!");
        }

        private IUnitOfWork CreateUnitOfWork(UnitOfWorkOptions options, bool needTransaction, CancellationToken cancellationToken)
        {
            if (options.SuspendTransaction)
            {
                needTransaction = false;
            }

            options.ConnectionString ??= this.configuration?.GetConnectionString(DefaultConnectionStringProvider.DefaultConnectionName);
            var connectionStringProvider = new DefaultConnectionStringProvider(options.ConnectionString);
            var timeMetricManager = this.metricProviderFactory.Create(options.DbQueryMetricInfo);
            return options.Factory(connectionStringProvider, needTransaction, timeMetricManager, this.logger, cancellationToken);
        }
    }
}
