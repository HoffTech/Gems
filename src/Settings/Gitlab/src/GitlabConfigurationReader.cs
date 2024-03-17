// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GitLabApiClient;
using GitLabApiClient.Models.Variables.Response;

namespace Gems.Settings.Gitlab
{
    public static class GitlabConfigurationReader
    {
        private const string AllEnvironmentsSymbol = "*";

        private enum Priority
        {
            Unset,
            SetByAllEnvironments,
            SetByExactEnvironment,
            SetByTag,
        }

        public static async Task<string> GetVariableValueByName(string variableName, string url, string token, int projectId, string prefix, List<string> prefixes)
        {
            var variables = await ReadFilteredByEnvironmentAsync(url, token, projectId, prefix, prefixes);
            return variables.FirstOrDefault(x => x.Key == variableName).Value;
        }

        public static async Task<Dictionary<string, string>> ReadFilteredByEnvironmentAsync(string url, string token, int projectId, string prefix, List<string> prefixes)
        {
            var gitlabVariables = await ReadAllAsync(url, token, projectId);
            var variableValuesForEnvironment = FilterVariablesByEnvironment(gitlabVariables, prefixes, prefix);
            return GitlabConfigurationParser.Parse(variableValuesForEnvironment, prefix);
        }

        private static (string prefix, string name) SplitToPrefixAndName(Variable variable, List<string> prefixes)
        {
            var variableName = variable.Key;
            foreach (var prefix in prefixes)
            {
                if (variableName.StartsWith(prefix))
                {
                    return (prefix, variableName.Remove(0, prefix.Length));
                }
            }

            return (string.Empty, variableName);
        }

        private static async Task<IList<Variable>> ReadAllAsync(string url, string token, int projectId)
        {
            var client = new GitLabClient(url, token);
            return await client.Projects.GetVariablesAsync(projectId);
        }

        private static Dictionary<string, string> FilterVariablesByEnvironment(IList<Variable> variables, List<string> prefixes, string targetPrefix)
        {
            var result = new Dictionary<string, PrioritizedVariable>();
            var targetPrefixWithoutUnderscore = targetPrefix.Remove(targetPrefix.Length - 1, 1);

            foreach (var variable in variables)
            {
                var (variablePrefix, variableName) = SplitToPrefixAndName(variable, prefixes);

                var variableWithPriority = new PrioritizedVariable
                {
                    VariableValue = variable.Value,
                    Priority = Priority.Unset
                };

                if (variablePrefix == targetPrefix)
                {
                    variableWithPriority.Priority = Priority.SetByTag;
                }
                else if (string.IsNullOrEmpty(variablePrefix) && string.Equals(variable.EnvironmentScope, targetPrefixWithoutUnderscore, StringComparison.CurrentCultureIgnoreCase))
                {
                    variableWithPriority.Priority = Priority.SetByExactEnvironment;
                }
                else if (string.IsNullOrEmpty(variablePrefix) && variable.EnvironmentScope == AllEnvironmentsSymbol)
                {
                    variableWithPriority.Priority = Priority.SetByAllEnvironments;
                }

                if (variableWithPriority.Priority == Priority.Unset)
                {
                    continue;
                }

                if (result.TryGetValue(variableName, out var foundVariable))
                {
                    if (foundVariable.Priority >= variableWithPriority.Priority)
                    {
                        continue;
                    }

                    foundVariable.VariableValue = variableWithPriority.VariableValue;
                    foundVariable.Priority = variableWithPriority.Priority;
                }
                else
                {
                    result.Add(variableName, variableWithPriority);
                }
            }

            return result.ToDictionary(pair => pair.Key, pair => pair.Value.VariableValue);
        }

        private class PrioritizedVariable
        {
            public string VariableValue { get; set; }

            public Priority Priority { get; set; }
        }
    }
}
