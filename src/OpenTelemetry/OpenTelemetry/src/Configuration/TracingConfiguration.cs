// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.OpenTelemetry.Configuration;

public class TracingConfiguration
{
    public string ServiceName { get; set; }

    public string ServiceVersion { get; set; }

    public string Endpoint { get; set; }

    public TracingConfigurationMssql Mssql { get; set; }

    public bool? Console { get; set; }

    public bool? Enabled { get; set; }

    public HttpConfiguration RequestIn { get; set; }

    public HttpConfiguration RequestOut { get; set; }

    public TracingConfigurationCommand Command { get; set; }

    public SimpleFilterConfiguration SourceFilter { get; set; }
}
