// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace Gems.Http.Samples.Mertics.RequestOverride.PingPong.SendPingCommand.PongApi.Dto
{
    public class PongDto
    {
        public string Secret { get; set; }

        [JsonIgnore]
        public Enum StatusCodeMetricType => PongMetricType.PongRequestMetric;
    }
}
