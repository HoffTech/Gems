// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Settings.Gitlab
{
    public class GitlabConfigurationSettings
    {
        public string GitlabUrl { get; set; }

        public string GitlabToken { get; set; }

        public int? GitlabProjectId { get; set; }

        public Dictionary<string, string> Prefixes { get; set; } = new Dictionary<string, string>
        {
            { "Development", "DEV_" },
            { "Staging", "STAGING_" },
            { "Production", "PROD_" },
        };
    }
}
