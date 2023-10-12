// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Gems.Context;
using Gems.Data.UnitOfWork.EntityFramework;

using Microsoft.Extensions.Logging;

namespace Gems.Data.UnitOfWork;

public class UnitOfWorkContextFactory : DefaultContextFactory
{
    private readonly IEnumerable<UnitOfWorkOptions> options;

    public UnitOfWorkContextFactory(
        IEnumerable<UnitOfWorkOptions> options,
        IContextAccessor contextAccessor,
        ILogger<IContextFactory> logger) : base(contextAccessor, logger)
    {
        this.options = options;
    }

    protected override void AddItems(IContext context)
    {
        base.AddItems(context);
        context.Items.TryAdd(UnitOfWorkProvider.UnitOfWorkMapName, this.options.ToDictionary(x => x.Key, _ => new ConcurrentDictionary<CancellationToken, IUnitOfWork>()));
        context.Items.TryAdd(EfUnitOfWorkProvider.EfUnitOfWorkMapName, new ConcurrentDictionary<Type, IEfUnitOfWork>());
    }
}
