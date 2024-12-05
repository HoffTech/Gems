// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Gems.Logging.Mvc.LogsCollector;

namespace Gems.Logging.Mvc.Behaviors
{
    public interface IRequestEndpointLogging
    {
        List<LogLevelOptions> GetLogLevelsByHttpStatus()
        {
            return null;
        }
    }
}
