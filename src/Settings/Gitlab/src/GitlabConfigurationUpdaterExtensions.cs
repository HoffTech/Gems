// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Reflection;

using Gems.Mvc.GenericControllers;
using Gems.Settings.Gitlab.Configuration;
using Gems.Settings.Gitlab.Handlers.Update;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Settings.Gitlab
{
    public static class GitlabConfigurationUpdaterExtensions
    {
        public static IServiceCollection AddGitlabConfigurationUpdater(
            this IServiceCollection services,
            Action<IGitlabConfigurationUpdaterSettingsBuilder> build = null)
        {
            return AddGitlabConfigurationUpdater(services, null, build);
        }

        public static IServiceCollection AddGitlabConfigurationUpdater(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IGitlabConfigurationUpdaterSettingsBuilder> build = null)
        {
            var builder = new GitlabConfigurationUpdaterSettingsBuilder();
            build?.Invoke(builder);
            services.AddSingleton(builder.Settings);

            var gitlabSettingsConfiguration = configuration?.GetGitlabSettingsConfiguration();
            if (gitlabSettingsConfiguration is not null)
            {
                services.AddSingleton(gitlabSettingsConfiguration);
            }

            if (gitlabSettingsConfiguration?.EnableBackgroundUpdater ?? true)
            {
                services.AddHostedService<GitlabConfigurationBackgroundUpdater>();
            }

            if (gitlabSettingsConfiguration?.EnableEndpointUpdater ?? false)
            {
                ControllerRegister.RegisterControllers(Assembly.GetExecutingAssembly());
                services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<GitlabSettingsUpdateCommand>());
            }

            services.AddSingleton<GitlabConfigurationUpdater>();
            services.AddSingleton<GitlabConfigurationValuesProvider>();
            return services;
        }
    }
}
