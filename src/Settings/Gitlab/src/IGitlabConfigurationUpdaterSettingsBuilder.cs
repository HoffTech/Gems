// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Settings.Gitlab
{
    public interface IGitlabConfigurationUpdaterSettingsBuilder
    {
        IGitlabConfigurationUpdaterSettingsBuilder AddPrefix(string env, string prefix);

        IGitlabConfigurationUpdaterSettingsBuilder ClearPrefixes();

        IGitlabConfigurationUpdaterSettingsBuilder SetErrorHandler(Action<IServiceProvider, Exception> handler);

        IGitlabConfigurationUpdaterSettingsBuilder SetGitlabProjectId(int value);

        IGitlabConfigurationUpdaterSettingsBuilder SetGitlabToken(string value);

        IGitlabConfigurationUpdaterSettingsBuilder SetGitlabUrl(string value);

        IGitlabConfigurationUpdaterSettingsBuilder SetValueChangedHandler(Action<IServiceProvider, string, string, string> handler);

        IGitlabConfigurationUpdaterSettingsBuilder UpdateInterval(TimeSpan value);
    }
}
