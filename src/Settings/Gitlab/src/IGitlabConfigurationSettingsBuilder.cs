// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Settings.Gitlab
{
    public interface IGitlabConfigurationSettingsBuilder
    {
        IGitlabConfigurationSettingsBuilder AddPrefix(string env, string prefix);

        IGitlabConfigurationSettingsBuilder ClearPrefixes();

        IGitlabConfigurationSettingsBuilder SetGitlabProjectId(int value);

        IGitlabConfigurationSettingsBuilder SetGitlabToken(string value);

        IGitlabConfigurationSettingsBuilder SetGitlabUrl(string value);
    }
}
