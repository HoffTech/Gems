// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.FeatureToggle;

namespace Gems.OpenTelemetry;

[FeatureToggles]
public class TracingFeatureFlags
{
    [FeatureToggle(defaultValue: false)]
    public bool Tracing { get; set; }
}
