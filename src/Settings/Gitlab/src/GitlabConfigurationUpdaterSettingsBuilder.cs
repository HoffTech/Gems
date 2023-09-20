// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Settings.Gitlab
{
    internal class GitlabConfigurationUpdaterSettingsBuilder : IGitlabConfigurationUpdaterSettingsBuilder
    {
        public GitlabConfigurationUpdaterSettingsBuilder()
        {
            this.Settings = new GitlabConfigurationUpdaterSettings();
        }

        public GitlabConfigurationUpdaterSettings Settings { get; }

        public IGitlabConfigurationUpdaterSettingsBuilder UpdateInterval(TimeSpan value)
        {
            this.Settings.UpdateInterval = value;
            return this;
        }

        public IGitlabConfigurationUpdaterSettingsBuilder AddPrefix(string env, string prefix)
        {
            this.Settings.Prefixes.Add(env, prefix);
            return this;
        }

        public IGitlabConfigurationUpdaterSettingsBuilder ClearPrefixes()
        {
            this.Settings.Prefixes.Clear();
            return this;
        }

        public IGitlabConfigurationUpdaterSettingsBuilder SetErrorHandler(Action<IServiceProvider, Exception> handler)
        {
            this.Settings.HandleError = handler;
            return this;
        }

        public IGitlabConfigurationUpdaterSettingsBuilder SetValueChangedHandler(Action<IServiceProvider, string, string, string> handler)
        {
            this.Settings.ValueChanged = handler;
            return this;
        }

        public IGitlabConfigurationUpdaterSettingsBuilder SetGitlabUrl(string value)
        {
            this.Settings.GitlabUrl = value;
            return this;
        }

        public IGitlabConfigurationUpdaterSettingsBuilder SetGitlabToken(string value)
        {
            this.Settings.GitlabToken = value;
            return this;
        }

        public IGitlabConfigurationUpdaterSettingsBuilder SetGitlabProjectId(int value)
        {
            this.Settings.GitlabProjectId = value;
            return this;
        }
    }
}
