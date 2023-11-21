// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace Gems.OpenTelemetry.Api.Dto
{
    internal class TraceRequest
    {
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; } = false;

        [JsonPropertyName("requestIn")]
        public TraceRequestSimpleFilter RequestIn { get; set; }

        [JsonPropertyName("requestOut")]
        public TraceRequestSimpleFilter RequestOut { get; set; }

        [JsonPropertyName("command")]
        public TraceRequestCommand Command { get; set; }

        [JsonPropertyName("source")]
        public TraceRequestSimpleFilter SourceFilter { get; set; }

        [JsonPropertyName("mssql")]
        public TraceRequestMssql Mssql { get; set; }
    }
}
