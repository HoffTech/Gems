// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.OpenTelemetry.GlobalOptions;

using OpenTelemetry.Trace;

namespace Gems.OpenTelemetry
{
    public class TracingSampler : Sampler
    {
        public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
        {
            return new SamplingResult(
                TracingGlobalOptions.Enabled && TracingGlobalOptions.SourceFilter.WildcardMatch(samplingParameters.Name) ?
                SamplingDecision.RecordAndSample :
                SamplingDecision.Drop);
        }
    }
}
