// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace Gems.OpenTelemetry.Api.Dto
{
    public class TraceRequestCommand
    {
        [JsonPropertyName("includeRequest")]
        public bool? IncludeRequest { get; set; } = false;

        [JsonPropertyName("includeResponse")]
        public bool? IncludeResponse { get; set; } = false;
    }
}
