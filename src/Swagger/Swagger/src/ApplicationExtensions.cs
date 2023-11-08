// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Swagger.Options;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Gems.Swagger
{
    /// <summary>
    /// Class with middleware extensions.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Class with middleware extensions.
        /// </summary>
        /// <param name="app">IApplicationBuilder.</param>
        /// <param name="configuration">IConfiguration.</param>
        public static void UseSwaggerApi(this IApplicationBuilder app, IConfiguration configuration)
        {
            var adOptions = configuration.GetSection(SwaggerOptions.Swagger).Get<SwaggerOptions>();

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    if (adOptions?.EnableAuthority ?? false)
                    {
                        options.OAuthClientId(adOptions.SwaggerKey);
                    }

                    options.SwaggerEndpoint("v1/swagger.json", adOptions?.SwaggerName ?? string.Empty);
                });
        }
    }
}
