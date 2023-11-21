// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.OpenTelemetry.Configuration
{
    public static class DefaultTracingConfiguration
    {
        public static List<string> RequestInUrlExclude { get; set; } = new List<string>
        {
            "/_*",
            "/swagger*",
            "/metrics",
            "/health",
            "/liveness",
            "/readiness",
            "/dashboard",
            "/quatz",
        };

        public static List<string> RequestOutUrlExclude { get; set; } = new List<string>();

        public static List<string> SourceFilter { get; set; } = new List<string>();

        public static List<string> MssqlCommandFilter { get; set; } = new List<string>();
    }
}
