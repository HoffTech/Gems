// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Logging.Mvc
{
    public struct SecureLoggerState
    {
        public List<string> ValueNames { get; set; }

        public object[] Values { get; set; }

        public string OriginalMessage { get; set; }
    }
}
