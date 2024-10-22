// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Gems.Settings.Gitlab;

using GitLabApiClient;
using GitLabApiClient.Internal.Paths;
using GitLabApiClient.Models.Variables.Response;

using Moq;

using NUnit.Framework;

public class GitlabConfigurationReaderTests
{
    [Test]
    [TestCase("DEV_", "1")]
    [TestCase("STAGE_", "3")]
    [TestCase("PROD_", "4")]
    public async Task ReadFilteredByEnvironmentAsync_DevEnvironment(string targetPrefix, string targetValue)
    {
        var projectId = 1;

        var mockGitlabClient = new Mock<IGitLabClient>();
        mockGitlabClient.Setup(x => x.Projects.GetVariablesAsync(It.IsAny<ProjectId>()))
            .ReturnsAsync(() => new List<Variable>
            {
#pragma warning disable SA1025, SA1001
                new Variable { Key = "DEV_VarName" , Value = "1", EnvironmentScope = "*"     },
                new Variable { Key = "VarName"     , Value = "2", EnvironmentScope = "Dev"   },
                new Variable { Key = "VarName"     , Value = "3", EnvironmentScope = "Stage" },
                new Variable { Key = "VarName"     , Value = "4", EnvironmentScope = "*"     },
#pragma warning restore SA1025, SA1001
            });

        var prefixes = new List<string> { "DEV_", "STAGING_", "PROD_" };
        var variables = await GitlabConfigurationReader.ReadFilteredByEnvironmentAsync(mockGitlabClient.Object, projectId, targetPrefix, prefixes);
        var variable = variables.FirstOrDefault();
        Assert.That(variable.Key, Is.EqualTo("VarName"));
        Assert.That(variable.Value, Is.EqualTo(targetValue));
    }
}
