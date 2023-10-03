// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Authentication.Jwt.Options;
using Gems.Mvc.Filters.Errors;
using Gems.Swagger.Filters;
using Gems.Swagger.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Gems.Swagger
{
    /// <summary>
    /// Class with middleware extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Class with middleware extensions.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="validationResultType">Type.</param>
        /// <param name="genericErrorType">Type.</param>
        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration, Type validationResultType = null, Type genericErrorType = null)
        {
            var swaggerOptions = configuration.GetSection(SwaggerOptions.Swagger).Get<SwaggerOptions>();

            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.CustomSchemaIds(type => type.ToString());
                options.DocumentFilter<PathPrefixInsertDocumentFilter>(swaggerOptions?.GitLabSwaggerPrefix ?? string.Empty);
                if (swaggerOptions?.EnableSchemaForErrorResponse ?? false)
                {
                    options.OperationFilter<DescriptionOperationFilter>();
                    options.OperationFilter<BadRequestOperationFilter>(validationResultType ?? typeof(BusinessErrorViewModel));
                    options.OperationFilter<InternalServerErrorOperationFilter>(genericErrorType ?? typeof(BusinessErrorViewModel));
                    options.OperationFilter<NotFoundOperationFilter>(validationResultType ?? typeof(BusinessErrorViewModel));
                }

                if (swaggerOptions?.EnableAuthority ?? false)
                {
                    var adOptions = configuration.GetSection(SwaggerOptions.Swagger).Get<ADOptions>();
                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri($"{adOptions.Authority}/oauth2/authorize/")
                            }
                        }
                    });
                    options.OperationFilter<AuthorizeCheckOperationFilter>();
                }
            });
        }
    }
}
