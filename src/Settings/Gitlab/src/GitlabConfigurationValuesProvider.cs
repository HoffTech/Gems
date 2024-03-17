// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Threading.Tasks;

using Gems.Text.Json;

namespace Gems.Settings.Gitlab;

public class GitlabConfigurationValuesProvider
{
    private readonly GitlabConfigurationUpdaterSettings settings;

    public GitlabConfigurationValuesProvider(GitlabConfigurationUpdaterSettings settings)
    {
        this.settings = settings;
    }

    public async Task<T> GetGitlabVariableValueByName<T>(string variableName)
    {
        var variableValue = await this.GetGitlabVariableValueByName(variableName);
        return variableValue is null ? default : variableValue.Deserialize<T>();
    }

    public async Task<string> GetGitlabVariableValueByName(string variableName)
    {
        if (!this.GetSettings(out var url, out var token, out var projectId, out var prefix))
        {
            return null;
        }

        return await GitlabConfigurationReader.GetVariableValueByName(variableName, url, token, Convert.ToInt32(projectId), prefix, this.settings.Prefixes.Values.ToList());
    }

    private bool GetSettings(out string url, out string token, out string projectId, out string prefix)
    {
        url = null;
        token = null;
        projectId = null;
        prefix = null;

        var aspNetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.IsNullOrWhiteSpace(aspNetEnvironment))
        {
            return false;
        }

        if (!this.settings.Prefixes.TryGetValue(aspNetEnvironment, out prefix))
        {
            return false;
        }

        url = this.settings.GitlabUrl ?? Environment.GetEnvironmentVariable("GITLAB_CONFIGURATION_URL");
        token = this.settings.GitlabToken ?? Environment.GetEnvironmentVariable("GITLAB_CONFIGURATION_TOKEN");
        projectId = this.settings.GitlabProjectId.HasValue ? this.settings.GitlabProjectId.ToString() : Environment.GetEnvironmentVariable("GITLAB_CONFIGURATION_PROJECTID");
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(projectId))
        {
            return false;
        }

        return true;
    }
}
