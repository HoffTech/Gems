using System;
using System.Text.Json.Serialization;

using Gems.Metrics.Http;

namespace Gems.Http.Samples.Mertics.RequestOverride.PingPong.SendPingCommand.PongApi.Dto
{
    public class PongDto
    {
        public string Secret { get; set; }

        [JsonIgnore]
        public Enum StatusCodeMetricType => PongMetricType.PongRequestMetric;
    }
}
