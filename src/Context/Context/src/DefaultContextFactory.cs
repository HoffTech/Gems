// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Gems.Context;

public class DefaultContextFactory : IContextFactory
{
    private readonly IContextAccessor contextAccessor;
    private readonly ILogger<IContextFactory> logger;

    public DefaultContextFactory(
        IContextAccessor contextAccessor,
        ILogger<IContextFactory> logger)
    {
        this.contextAccessor = contextAccessor;
        this.logger = logger;
    }

    public virtual IContext Create()
    {
        var context = new DefaultContext();
        this.AddItems(context);
        this.contextAccessor.Context = context;
        this.logger.LogTrace($"ContextCreated: {context.GetHashCode()}");
        return context;
    }

    public virtual async ValueTask DisposeAsync()
    {
        var context = this.contextAccessor.Context;
        this.contextAccessor.Context = null;

        foreach (var item in context.Items)
        {
            if (item.Value is IDisposable disposableItem)
            {
                disposableItem.Dispose();
            }

            if (item.Value is IAsyncDisposable asyncDisposableItem)
            {
                await asyncDisposableItem.DisposeAsync();
            }
        }

        this.logger.LogTrace($"ContextDisposed: {context.GetHashCode()}");
    }

    protected virtual void AddItems(IContext context)
    {
    }
}
