// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.OpenTelemetry.GlobalOptions;

using Microsoft.Extensions.Configuration;

namespace Gems.OpenTelemetry.Configuration
{
    internal static class IConfigurationExtensions
    {
        private const string SectionName = "Tracing";

        public static TracingConfiguration GetTracingConfiguration(this IConfiguration configuration)
        {
            var tracingConfigurationSection = configuration.GetSection(SectionName);
            var tracingConfiguration = new TracingConfiguration();
            tracingConfigurationSection.Bind(tracingConfiguration);
            return tracingConfiguration;
        }

        public static TracingConfiguration GetAndUpdateTracingConfiguration(this IConfiguration configuration)
        {
            var tracingConfiguration = configuration.GetTracingConfiguration();
            TracingGlobalOptions.Configure(tracingConfiguration);
            return tracingConfiguration;
        }
    }
}
