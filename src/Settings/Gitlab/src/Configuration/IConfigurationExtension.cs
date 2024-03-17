// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;

namespace Gems.Settings.Gitlab.Configuration;

internal static class IConfigurationExtension
{
    private const string SectionName = "GitlabSettings";

    public static GitlabSettingsConfiguration GetGitlabSettingsConfiguration(this IConfiguration configuration)
    {
        var gitlabConfigurationSection = configuration.GetSection(SectionName);
        var gitlabConfiguration = new GitlabSettingsConfiguration();
        gitlabConfigurationSection.Bind(gitlabConfiguration);
        return gitlabConfiguration;
    }
}
