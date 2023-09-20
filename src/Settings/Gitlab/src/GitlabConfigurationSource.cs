// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;

namespace Gems.Settings.Gitlab
{
    internal class GitlabConfigurationSource : IConfigurationSource
    {
        private readonly GitlabConfigurationSettings settings;

        public GitlabConfigurationSource(GitlabConfigurationSettings settings)
        {
            this.settings = settings;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new GitlabConfigurationProvider(this.settings);
        }
    }
}
