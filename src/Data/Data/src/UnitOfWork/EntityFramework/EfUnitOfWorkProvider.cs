// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gems.Data.UnitOfWork.EntityFramework;

public class EfUnitOfWorkProvider : IEfUnitOfWorkProvider
{
    /// <summary>
    /// Ключ, по которому хранится EfUnitOfWorkMap в контексте.
    /// </summary>
    public const string EfUnitOfWorkMapName = "__EfUnitOfWorkMap__";

    private readonly IContextAccessor contextAccessor;
    private readonly ILogger<DbContextProvider> logger;
    private readonly ConcurrentDictionary<Type, object> factories;

    public EfUnitOfWorkProvider(IContextAccessor contextAccessor, ILogger<DbContextProvider> logger, ConcurrentDictionary<Type, object> factories)
    {
        this.contextAccessor = contextAccessor;
        this.logger = logger;
        this.factories = factories;
    }

    public List<EfUnitOfWork> GetUnitOfWorks(bool needTransaction, CancellationToken cancellationToken)
    {
        return this.factories.Keys.Select(dbContextType => this.GetUnitOfWork(dbContextType, needTransaction, cancellationToken)).ToList();
    }

    public EfUnitOfWork GetUnitOfWork<TDbContext>(CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        var efUnitOfWork = this.GetUnitOfWork(typeof(TDbContext), false, cancellationToken);
        this.SetDbContextFactory<TDbContext>(efUnitOfWork);
        return efUnitOfWork;
    }

    public async Task RemoveUnitOfWorksAsync()
    {
        var context = this.contextAccessor.Context;
        var efUnitOfWorkMap = this.GetEfUnitOfWorkMapFromContext(context);
        foreach (var (_, efUnitOfWork) in efUnitOfWorkMap)
        {
            await efUnitOfWork.DisposeAsync();
        }

        context.Items.TryUpdate(EfUnitOfWorkMapName, new ConcurrentDictionary<Type, IEfUnitOfWork>(), efUnitOfWorkMap);
    }

    private EfUnitOfWork GetUnitOfWork(Type dbContextType, bool needTransaction, CancellationToken cancellationToken)
    {
        var context = this.contextAccessor.Context;
        var efUnitOfWorkMap = this.GetEfUnitOfWorkMapFromContext(context);
        if (efUnitOfWorkMap.TryGetValue(dbContextType, out var efUnitOfWorkAsInterface) && efUnitOfWorkAsInterface is EfUnitOfWork efUnitOfWork)
        {
            return efUnitOfWork;
        }

        efUnitOfWork = this.CreateEfUnitOfWorkAsync(needTransaction, cancellationToken);
        efUnitOfWorkMap.TryAdd(dbContextType, efUnitOfWork);
        return efUnitOfWork;
    }

    private void SetDbContextFactory<TDbContext>(EfUnitOfWork efUnitOfWork) where TDbContext : DbContext
    {
        if (efUnitOfWork.DbContextFactory == null)
        {
            var dbContextFactory = this.GetDbContextFactory<TDbContext>();
            efUnitOfWork.DbContextFactory = async token => await dbContextFactory.CreateDbContextAsync(token);
        }
    }

    private EfUnitOfWork CreateEfUnitOfWorkAsync(bool needTransaction, CancellationToken cancellationToken)
    {
        return new EfUnitOfWork(needTransaction, cancellationToken);
    }

    private IDbContextFactory<TDbContext> GetDbContextFactory<TDbContext>()
        where TDbContext : DbContext
    {
        if (this.factories.TryGetValue(typeof(TDbContext), out var factory) &&
            factory is IDbContextFactory<TDbContext> dbContextFactory)
        {
            return dbContextFactory;
        }

        throw new InvalidDataException($"Не была зарегистрирована фабрика IDbContextFactory<{typeof(TDbContext).Name}>");
    }

    private ConcurrentDictionary<Type, IEfUnitOfWork> GetEfUnitOfWorkMapFromContext(IContext context)
    {
        this.logger.LogTrace($"Context: {context.GetHashCode()}");
        if (context.Items.TryGetValue(EfUnitOfWorkMapName, out var efUnitOfWorkMapAsObject) &&
            efUnitOfWorkMapAsObject is ConcurrentDictionary<Type, IEfUnitOfWork> efUnitOfWorkMap)
        {
            this.logger.LogTrace($"EfUnitOfWorkMap: {efUnitOfWorkMap.GetHashCode()}");
            return efUnitOfWorkMap;
        }

        throw new InvalidOperationException("Не удалось получить efUnitOfWorkMap из контекста IContext.");
    }
}
