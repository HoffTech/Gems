// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.OpenTelemetry.Configuration;

namespace Gems.OpenTelemetry.GlobalOptions
{
    public static class TracingGlobalOptions
    {
        public static string ServiceName { get; set; } = string.Empty;

        public static bool Enabled { get; set; } = false;

        public static SimpleFilter RequestInUrlFilter { get; set; } = new SimpleFilter();

        public static SimpleFilter RequestOutUrlFilter { get; set; } = new SimpleFilter();

        public static SimpleFilter SourceFilter { get; set; } = new SimpleFilter();

        public static SimpleFilter MssqlCommandFilter { get; set; } = new SimpleFilter();

        public static bool IncludeCommandRequest { get; set; } = false;

        public static bool IncludeCommandResponse { get; set; } = false;

        internal static void Configure(TracingConfiguration o)
        {
            RequestInUrlFilter.Assign(o?.RequestIn?.UrlFilter, DefaultTracingConfiguration.RequestInUrlExclude);
            RequestOutUrlFilter.Assign(o?.RequestOut?.UrlFilter, DefaultTracingConfiguration.RequestOutUrlExclude);
            SourceFilter.Assign(o?.SourceFilter, DefaultTracingConfiguration.SourceFilter);
            MssqlCommandFilter.Assign(o?.Mssql?.CommandFilter, DefaultTracingConfiguration.MssqlCommandFilter);

            Enabled = o?.Enabled ?? Enabled;
            IncludeCommandRequest = o?.Command?.IncludeRequest ?? IncludeCommandRequest;
            IncludeCommandResponse = o?.Command?.IncludeResponse ?? IncludeCommandResponse;
        }
    }
}
