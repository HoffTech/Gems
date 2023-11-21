// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace Gems.OpenTelemetry.Api.Dto
{
    public class TraceRequestSimpleFilter
    {
        [JsonPropertyName("include")]
        public List<string> Include { get; set; }

        [JsonPropertyName("exclude")]
        public List<string> Exclude { get; set; }
    }
}
