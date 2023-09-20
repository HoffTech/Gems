// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

using GitLabApiClient;

namespace Gems.Settings.Gitlab
{
    internal static class GitlabConfigurationReader
    {
        public static async Task<Dictionary<string, string>> ReadAsync(
            string url,
            string token,
            int projectId,
            string prefix)
        {
            var result = new Dictionary<string, string>();
            var client = new GitLabClient(url, token);
            var variables = await client.Projects.GetVariablesAsync(projectId);
            foreach (var variable in variables)
            {
                if (variable.Key.StartsWith(prefix))
                {
                    result.Add(variable.Key, variable.Value);
                }
            }

            return result;
        }
    }
}
