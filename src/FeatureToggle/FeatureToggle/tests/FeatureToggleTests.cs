// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using NLog;
using NLog.Web;

using NUnit.Framework;

namespace Gems.FeatureToggle.Tests;

[FeatureToggles]
public class SomeToggles
{
    public bool EnableOneFeature { get; set; }

    [FeatureToggle(defaultValue: true)]
    public bool EnableAnotherFeature { get; set; }
}

[FeatureToggles]
public class AnotherToggles
{
    [FeatureToggle("enable_one_feature", defaultValue: true)]
    public bool AnotherName { get; set; }

    public bool EnableAnotherFeature { get; set; }
}

public class FeatureToggleTests
{
    [Test]
    [TestCase("check default toggles", true, false)]
    [TestCase("check default toggles", false, false)]
    [TestCase("check that all configured toggles enabled", true, true)]
    [TestCase("check that all configured toggles disabled", false, true)]
    public async Task CheckDefaultFeatureFlags(string testCase, bool toglesValue, bool dataAvalible)
    {
        var sp = BuildTestScope(toglesValue, dataAvalible);

        var fts = sp.GetRequiredService<IFeatureToggleService>();
        await fts.StartAsync(CancellationToken.None);
        foreach (var ft in fts.FeatureToggles)
        {
            Debug.WriteLine(ft);
        }

        var someToggles = sp.GetRequiredService<SomeToggles>();
        var anotherTogles = sp.GetRequiredService<AnotherToggles>();

        if (dataAvalible)
        {
            fts.IsEnabled("enable_one_feature")
                .Should().Be(toglesValue);
            fts.IsEnabled("enable_another_feature")
                .Should().Be(toglesValue);

            someToggles.EnableAnotherFeature
                .Should().Be(toglesValue, $"{testCase}: value SomeTogles.EnableAnotherFeature should be {toglesValue}");
            someToggles.EnableOneFeature
                .Should().Be(toglesValue, $"{testCase}: value SomeTogles.EnableOneFeature should be {toglesValue}");

            anotherTogles.EnableAnotherFeature
                .Should().Be(toglesValue, $"{testCase}: value AnotherTogles.EnableAnotherFeature should be {toglesValue}");
            anotherTogles.AnotherName
                .Should().Be(toglesValue, $"{testCase}: value AnotherTogles.AnotherName should be {toglesValue}");
        }
        else
        {
            someToggles.EnableAnotherFeature
                .Should().BeTrue($"{testCase}: default value SomeTogles.EnableAnotherFeature for first initialization should be true");
            someToggles.EnableOneFeature
                .Should().BeFalse($"{testCase}: default value SomeTogles.EnableOneFeature for first initialization should be true");

            anotherTogles.EnableAnotherFeature
                .Should().BeFalse($"{testCase}: default value AnotherTogles.EnableAnotherFeature for first initialization should be BeFalse");
            anotherTogles.AnotherName
                .Should().BeTrue($"{testCase}: default value AnotherTogles.AnotherName for first initialization should be true");
        }
    }

    private static ServiceProvider BuildTestScope(bool toglesValue = true, bool dataAvalible = false)
    {
        LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        using var configStream = new MemoryStream(
            Encoding.UTF8.GetBytes(
            $@"{{
                ""FeatureToggle"" :
                {{
                    ""Token"" : ""{Guid.NewGuid():N}"",
                    ""Environment"" : ""dev"",
                    ""Url"" : ""https://test.io"",
                    ""FetchTogglesInterval"" : ""00:00:05"",
                    ""EnableBootstrapLoading"" : false,
                    ""SynchronousInitialization"" : {dataAvalible.ToString().ToLowerInvariant()}
                }}
            }}"));

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(configStream)
            .Build();

        var sc = new ServiceCollection();
        sc.AddLogging(builder =>
            builder.AddDebug());

        ConfigureTestFeatureToggleClient(toglesValue, sc, configuration, dataAvalible);

        return sc.BuildServiceProvider();
    }

    private static void ConfigureTestFeatureToggleClient(
        bool toglesValue,
        ServiceCollection sc,
        IConfigurationRoot configuration,
        bool dataAvalible)
    {
        var togglesJson =
@$"{{
    ""version"":100,
    ""features"":[
        {{
            ""name"":""enable_one_feature"",
            ""description"":""asdf"",
            ""enabled"":{toglesValue.ToString().ToLowerInvariant()},
            ""strategies"":[{{""name"":""default"",""parameters"":{{ }} }}]
        }},
        {{
            ""name"":""enable_another_feature"",
            ""description"":""asdf"",
            ""enabled"":{toglesValue.ToString().ToLowerInvariant()},
            ""strategies"":[{{""name"":""default"",""parameters"":{{ }} }}]
        }}
    ]
}}";

        var mockHttp = new Mock<HttpClient>();

        if (dataAvalible)
        {
            mockHttp
                .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    var response = new HttpResponseMessage
                    {
                        Content = new StringContent(togglesJson, Encoding.UTF8, "application/json")
                    };

                    response.Headers.ETag =
                        new System.Net.Http.Headers.EntityTagHeaderValue(
                            @$"""{Guid.NewGuid():N}""", true);
                    return response;
                });
        }

        sc.ConfigureFeatureToggle<FeatureToggleTests>(
            configuration,
            opt => opt.CustomHttpClientBuilder = _ => mockHttp.Object);
    }
}
