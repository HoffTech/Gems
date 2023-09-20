// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;

namespace Gems.Logging.Mvc.Middlewares
{
    public static class EndpointLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseEndpointLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<EndpointLoggingMiddleware>();
        }
    }
}
