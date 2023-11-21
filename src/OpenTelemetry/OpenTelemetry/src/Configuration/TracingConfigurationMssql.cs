// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.OpenTelemetry.Configuration;

public class TracingConfigurationMssql
{
    public bool? SetDbStatementForStoredProcedure { get; set; }

    public bool? SetDbStatementForText { get; set; }

    public bool? RecordException { get; set; }

    public SimpleFilterConfiguration CommandFilter { get; set; }
}
