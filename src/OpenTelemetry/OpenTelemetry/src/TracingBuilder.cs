// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Npgsql;

using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Trace;

namespace Gems.OpenTelemetry;

public class TracingBuilder
{
    public Action<TracerProviderBuilder> TracerProviderConfigurator { get; set; }

    public Action<OtlpExporterOptions> OtlpExporterConfigurator { get; set; }

    public Action<SqlClientInstrumentationOptions> MssqlConfigurator { get; set; }

    public Action<NpgsqlTracingOptions> NpgsqlConfigurator { get; set; }

    public Action<HttpClientInstrumentationOptions> HttpClientConfigurator { get; set; }

    public Action<AspNetCoreInstrumentationOptions> AspNetCoreConfigurator { get; set; }

    public string ServiceName { get; set; } = default;

    public string ServiceVersion { get; set; } = default;

    public bool? EnableTracing { get; set; }
}
