// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Integration;

namespace Gems.TestInfrastructure.Quartz.Integration
{
    public static class TestApplicationBuilderQuartzExtensions
    {
        public static ITestApplicationBuilder RemoveQuartzHostedService(this ITestApplicationBuilder builder)
        {
            return builder.RemoveServiceImplementationByName("QuartzHostedService");
        }
    }
}
