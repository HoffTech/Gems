// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.Extensions.Configuration;

namespace Gems.Settings.Gitlab
{
    public static class GitlabConfigurationProviderExtensions
    {
        public static IConfigurationBuilder AddGitlabConfiguration(
            this IConfigurationBuilder configurationBuilder,
            Action<IGitlabConfigurationSettingsBuilder> build = null)
        {
            var builder = new GitlabConfigurationSettingsBuilder();
            build?.Invoke(builder);
            configurationBuilder.Add(new GitlabConfigurationSource(builder.Settings));
            return configurationBuilder;
        }
    }
}
