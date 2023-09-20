// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Settings.Gitlab
{
    internal class GitlabConfigurationSettingsBuilder : IGitlabConfigurationSettingsBuilder
    {
        public GitlabConfigurationSettingsBuilder()
        {
            this.Settings = new GitlabConfigurationSettings();
        }

        public GitlabConfigurationSettings Settings { get; }

        public IGitlabConfigurationSettingsBuilder AddPrefix(string env, string prefix)
        {
            this.Settings.Prefixes.Add(env, prefix);
            return this;
        }

        public IGitlabConfigurationSettingsBuilder ClearPrefixes()
        {
            this.Settings.Prefixes.Clear();
            return this;
        }

        public IGitlabConfigurationSettingsBuilder SetGitlabUrl(string value)
        {
            this.Settings.GitlabUrl = value;
            return this;
        }

        public IGitlabConfigurationSettingsBuilder SetGitlabToken(string value)
        {
            this.Settings.GitlabToken = value;
            return this;
        }

        public IGitlabConfigurationSettingsBuilder SetGitlabProjectId(int value)
        {
            this.Settings.GitlabProjectId = value;
            return this;
        }
    }
}
