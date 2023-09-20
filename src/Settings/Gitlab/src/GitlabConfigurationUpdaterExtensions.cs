// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.Extensions.DependencyInjection;

namespace Gems.Settings.Gitlab
{
    public static class GitlabConfigurationUpdaterExtensions
    {
        public static IServiceCollection AddGitlabConfigurationUpdater(
            this IServiceCollection services,
            Action<IGitlabConfigurationUpdaterSettingsBuilder> build = null)
        {
            var builder = new GitlabConfigurationUpdaterSettingsBuilder();
            build?.Invoke(builder);
            services.AddSingleton(builder.Settings);
            services.AddHostedService<GitlabConfigurationUpdater>();
            return services;
        }
    }
}
