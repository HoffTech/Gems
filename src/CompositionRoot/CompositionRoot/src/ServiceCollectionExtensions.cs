// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Autofac;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.CompositionRoot;

public static class ServiceCollectionExtensions
{
    public static void ConfigureCompositionRoot<TFromAssemblyContaining>(this IServiceCollection services, IConfiguration configuration, Action<CompositionRootOptions> configureOptions = null)
    {
        var compositionRootBuilder = new CompositionRootBuilder<TFromAssemblyContaining>(services, configuration, configureOptions);
        compositionRootBuilder.Build();
    }

    public static void ConfigureAutofacCompositionRoot<TFromAssemblyContaining>(this ContainerBuilder containerBuilder, Action<AutofacCompositionRootOptions> configureOptions = null)
    {
        var compositionRootBuilder = new AutofacCompositionRootBuilder<TFromAssemblyContaining>(containerBuilder, configureOptions);
        compositionRootBuilder.Build();
    }
}
