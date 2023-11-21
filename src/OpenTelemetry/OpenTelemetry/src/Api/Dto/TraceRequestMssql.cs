// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace Gems.OpenTelemetry.Api.Dto
{
    internal class TraceRequestMssql
    {
        [JsonPropertyName("command")]
        public TraceRequestSimpleFilter CommandFilter { get; set; }
    }
}
