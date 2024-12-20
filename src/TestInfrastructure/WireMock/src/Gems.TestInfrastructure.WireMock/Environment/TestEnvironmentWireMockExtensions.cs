// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Environment;

using WireMock.Server;

namespace Gems.TestInfrastructure.WireMock.Environment
{
    public static class TestEnvironmentWireMockExtensions
    {
        public static WireMockServer WireMockServer(this ITestEnvironment env, string name)
            => env.Component<WireMockServer>(name);

        public static string WireMockServerUrl(this ITestEnvironment env, string name)
            => env.Component<WireMockServer>(name).Url;

        public static Uri WireMockServerUri(this ITestEnvironment env, string name)
            => new Uri(env.Component<WireMockServer>(name).Url);
    }
}
