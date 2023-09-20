// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Gems.Context
{
    public static class ServiceCollectionExtensions
    {
        public static void AddContext<TContextFactory>(this IServiceCollection services)
            where TContextFactory : class, IContextFactory
        {
            services.AddSingleton<IContextAccessor, ContextAccessor>();
            services.AddSingleton<IContextFactory, TContextFactory>();
        }
    }
}
