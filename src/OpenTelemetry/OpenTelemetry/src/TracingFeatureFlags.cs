// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.FeatureToggle;

namespace Gems.OpenTelemetry;

[FeatureToggles]
public class TracingFeatureFlags
{
    [FeatureToggle(featureName: "dev_tracing", defaultValue: false)]
    public bool DevTracing { get; set; }

    [FeatureToggle(featureName: "stage_tracing", defaultValue: false)]
    public bool StageTracing { get; set; }

    [FeatureToggle(featureName: "prod_tracing", defaultValue: false)]
    public bool ProdTracing { get; set; }

    [FeatureToggle(featureName: "tracing", defaultValue: false)]
    public bool Tracing { get; set; }
}
