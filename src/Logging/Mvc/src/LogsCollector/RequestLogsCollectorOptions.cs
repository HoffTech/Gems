// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace Gems.Logging.Mvc.LogsCollector;

public class RequestLogsCollectorOptions
{
    public const string Name = "RequestLogsCollector";

    public static List<LogLevelOptions> DefaultLogLevelsByHttpStatus { get; } = new List<LogLevelOptions>
    {
        new LogLevelOptions { Status = 200, Level = LogLevel.Information },
        new LogLevelOptions { Status = 400, Level = LogLevel.Error },
        new LogLevelOptions { Status = 500, Level = LogLevel.Error }
    };

    public List<LogLevelOptions> LogLevelsByHttpStatus { get; set; }
}
