// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

using Gems.Swagger.Options;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gems.Swagger.Filters
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        private readonly IOptions<SwaggerOptions> option;

        public AuthorizeCheckOperationFilter(IOptions<SwaggerOptions> option)
        {
            this.option = option ?? throw new ArgumentNullException(nameof(option));
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttr = context.MethodInfo.GetCustomAttributes(false).OfType<AuthorizeAttribute>().FirstOrDefault()
                           ?? context.MethodInfo.DeclaringType?.GetCustomAttributes(false).OfType<AuthorizeAttribute>().FirstOrDefault();

            if (authAttr == null)
            {
                return;
            }

            if (authAttr.Roles != null)
            {
                var roles = string.Join(" | ", authAttr.Roles.Split(",").OrderBy(x => x));
                operation.Summary += $" [{roles}]";
            }

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        }

                    ] = new[] { this.option.Value.SwaggerSchema }
                }
            };
        }
    }
}
