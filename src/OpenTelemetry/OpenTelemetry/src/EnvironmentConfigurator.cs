// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using OpenTelemetry.Exporter;

namespace Gems.OpenTelemetry;

internal class EnvironmentConfigurator
{
    public static void Configure(OtlpExporterOptions otlp)
    {
        var otlpEndpoint = Environment.GetEnvironmentVariable("TRACE_ENDPOINT");
        if (!string.IsNullOrEmpty(otlpEndpoint))
        {
            otlp.Endpoint = new Uri(otlpEndpoint);
        }
    }
}
