// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.FeatureToggle;

public static class FeatureToggleServiceCollectionExtensions
{
    public static IServiceCollection ConfigureFeatureToggle<TFromAssemblyContaining>(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<FeatureToggleOptions> configureOptions = null)
    {
        var featureToggleConfigurationSection = configuration.GetSection("FeatureToggle");
        var featureToggleOptions = featureToggleConfigurationSection.Get<FeatureToggleOptions>();
        if (featureToggleOptions == null)
        {
            return serviceCollection;
        }

        serviceCollection.Configure<FeatureToggleOptions>(featureToggleConfigurationSection);
        if (configureOptions != null)
        {
            serviceCollection.Configure(configureOptions);
        }

        var featureTogglesTypes = typeof(TFromAssemblyContaining).Assembly.DefinedTypes
            .Where(dt =>
                dt.CustomAttributes.Any(
                    x => x.AttributeType == typeof(FeatureTogglesAttribute)))
            .Select(ftt => ftt.AsType())
            .ToArray();

        foreach (var ftt in featureTogglesTypes)
        {
            serviceCollection.AddSingleton(ftt);
        }

        serviceCollection.AddSingleton<IFeatureToggleService>(serviceProvider =>
            new FeatureToggleService(
                serviceProvider,
                serviceProvider.GetRequiredService<IOptions<FeatureToggleOptions>>(),
                serviceProvider.GetRequiredService<ILogger<FeatureToggleService>>(),
                featureTogglesTypes));

        serviceCollection.AddHostedService(sp =>
            sp.GetRequiredService<IFeatureToggleService>());

        return serviceCollection;
    }
}
