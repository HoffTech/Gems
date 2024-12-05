// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

using Microsoft.Extensions.Logging;

namespace Gems.Logging.Mvc.LogsCollector;

public class LogLevelOptions
{
    public int Status { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LogLevel Level { get; set; }
}
