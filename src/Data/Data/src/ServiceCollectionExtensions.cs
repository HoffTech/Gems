// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Dapper;

using Gems.Context;
using Gems.Data.UnitOfWork;
using Gems.Data.UnitOfWork.EntityFramework;
using Gems.Metrics.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gems.Data
{
    /// <summary>
    /// Class with middleware extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void AddDbContextProvider(this IServiceCollection services)
        {
            services.AddCommonUnitOfWorkServices();
        }

        internal static void AddCommonUnitOfWorkServices(this IServiceCollection services)
        {
            if (services.All(d => d.ServiceType != typeof(UnitOfWorkContextFactory)))
            {
                services.AddContext<UnitOfWorkContextFactory>();
                SqlMapper.AddTypeHandler(new SqlMappers.DapperSqlDateOnlyTypeHandler());
            }

            if (services.All(d => d.ServiceType != typeof(IUnitOfWorkProvider)))
            {
                services.AddSingleton<IUnitOfWorkProvider, UnitOfWorkProvider>();
            }

            if (services.All(d => d.ServiceType != typeof(ITimeMetricProviderFactory)))
            {
                services.AddSingleton<ITimeMetricProviderFactory, TimeMetricProviderFactory>();
            }

            if (services.All(d => d.ServiceType != typeof(IEfUnitOfWorkProvider)))
            {
                services.AddSingleton<IEfUnitOfWorkProvider>(EfUnitOfWorkProviderFactory);
            }

            if (services.All(d => d.ServiceType != typeof(IDbContextProvider)))
            {
                services.AddSingleton<IDbContextProvider, DbContextProvider>();
            }
        }

        private static EfUnitOfWorkProvider EfUnitOfWorkProviderFactory(IServiceProvider provider)
        {
            var contextAccessor = provider.GetService<IContextAccessor>();
            var logger = provider.GetService<ILogger<DbContextProvider>>();
            var dbOptionsList = provider.GetService<IEnumerable<DbContextOptions>>();

            var factories = new ConcurrentDictionary<Type, object>();
            foreach (var dbOptions in dbOptionsList)
            {
                var factory = provider.GetService(typeof(IDbContextFactory<>).MakeGenericType(dbOptions.ContextType));
                if (factory == null)
                {
                    throw new InvalidDataException($"Не была зарегистрирована фабрика IDbContextFactory<{dbOptions.ContextType.Name}>");
                }

                factories.TryAdd(dbOptions.ContextType, factory);
            }

            return new EfUnitOfWorkProvider(contextAccessor, logger, factories);
        }
    }
}
