// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Autofac;

using Gems.Logging.Mvc;

using Microsoft.Extensions.Logging;

namespace Gems.CompositionRoot;

public class AutofacCompositionRootBuilder<TFromAssemblyContaining>
{
    private readonly ContainerBuilder containerBuilder;
    private readonly Action<AutofacCompositionRootOptions> configureOptions;

    public AutofacCompositionRootBuilder(ContainerBuilder containerBuilder, Action<AutofacCompositionRootOptions> configureOptions = null)
    {
        this.containerBuilder = containerBuilder;
        this.configureOptions = configureOptions;
    }

    public void Build()
    {
        var options = new AutofacCompositionRootOptions();
        this.configureOptions?.Invoke(options);

        (options.AddSecureLoggerDecorators ?? this.AddSecureLoggerDecorators)();
    }

    private void AddSecureLoggerDecorators()
    {
        this.containerBuilder.RegisterGenericDecorator(typeof(SecureLogger<>), typeof(ILogger<>));
    }
}
