// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace Gems.Logging.Mvc.LogsCollector
{
    public interface IRequestLogsCollectorFactory
    {
        RequestLogsCollector Create();

        RequestLogsCollector Create(ILogger logger);

        RequestLogsCollector Create(ILogger logger, List<List<LogLevelOptions>> logLevelsByHttpStatus);
    }
}
