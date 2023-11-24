// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.OpenTelemetry.Api;
using Gems.OpenTelemetry.Configuration;
using Gems.OpenTelemetry.GlobalOptions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Gems.OpenTelemetry
{
    public static class WebApplicationExtensions
    {
        public static IEndpointRouteBuilder MapTracingApi(this IEndpointRouteBuilder endpoints, TracingApiOptions options = null)
        {
            var endpoint = options?.Endpoint ?? "/trace";
            DefaultTracingConfiguration.RequestInUrlExclude.Add(endpoint);
            endpoints.MapPost(
                endpoint,
                static async ctx =>
                {
                    var request = await ctx.Request.ReadFromJsonAsync<Api.Dto.TraceRequest>();
                    TracingGlobalOptions.Enabled = request?.Enabled ?? TracingGlobalOptions.Enabled;
                    TracingGlobalOptions.RequestInUrlFilter.Include = request?.RequestIn?.Include ?? TracingGlobalOptions.RequestInUrlFilter.Include;
                    TracingGlobalOptions.RequestInUrlFilter.Exclude = request?.RequestIn?.Exclude ?? TracingGlobalOptions.RequestInUrlFilter.Exclude;
                    TracingGlobalOptions.RequestOutUrlFilter.Include = request?.RequestOut?.Include ?? TracingGlobalOptions.RequestOutUrlFilter.Include;
                    TracingGlobalOptions.RequestOutUrlFilter.Exclude = request?.RequestOut?.Exclude ?? TracingGlobalOptions.RequestOutUrlFilter.Exclude;
                    TracingGlobalOptions.SourceFilter.Include = request?.SourceFilter?.Include ?? TracingGlobalOptions.SourceFilter.Include;
                    TracingGlobalOptions.SourceFilter.Exclude = request?.SourceFilter?.Exclude ?? TracingGlobalOptions.SourceFilter.Exclude;
                    TracingGlobalOptions.MssqlCommandFilter.Include = request?.Mssql?.CommandFilter?.Include ?? TracingGlobalOptions.MssqlCommandFilter.Include;
                    TracingGlobalOptions.MssqlCommandFilter.Exclude = request?.Mssql?.CommandFilter?.Exclude ?? TracingGlobalOptions.MssqlCommandFilter.Exclude;
                    TracingGlobalOptions.IncludeCommandRequest = request?.Command?.IncludeRequest ?? TracingGlobalOptions.IncludeCommandRequest;
                    TracingGlobalOptions.IncludeCommandResponse = request?.Command?.IncludeResponse ?? TracingGlobalOptions.IncludeCommandResponse;
                    ctx.Response.StatusCode = 201;
                });
            return endpoints;
        }
    }
}
