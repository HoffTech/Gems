// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;

using Gems.Authentication.Jwt.Options;
using Gems.Mvc.Filters.Errors;
using Gems.Swagger.Filters;
using Gems.Swagger.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

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
                if (swaggerOptions?.EnableAnnotations ?? true)
                {
                    options.EnableAnnotations();
                }

                if (swaggerOptions?.UseOneOfForPolymorphism ?? false)
                {
                    options.UseOneOfForPolymorphism();
                }

                foreach (var xmlCommentPath in swaggerOptions?.IncludeXmlComments ?? new List<string>())
                {
                    options.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlCommentPath), true);
                }

                options.CustomSchemaIds(type => type.ToString());
                options.DocumentFilter<PathPrefixInsertDocumentFilter>(swaggerOptions?.GitLabSwaggerPrefix ?? string.Empty);
                if (swaggerOptions?.EnableSchemaForErrorResponse ?? false)
                {
                    options.OperationFilter<DescriptionOperationFilter>();
                    options.OperationFilter<BadRequestOperationFilter>(validationResultType ?? typeof(BusinessErrorViewModel));
                    options.OperationFilter<InternalServerErrorOperationFilter>(genericErrorType ?? typeof(BusinessErrorViewModel));
                    options.OperationFilter<NotFoundOperationFilter>(validationResultType ?? typeof(BusinessErrorViewModel));
                }

                if (swaggerOptions?.SwaggerDoc != null)
                {
                    foreach (var doc in swaggerOptions.SwaggerDoc)
                    {
                        options.SwaggerDoc(doc.Key, doc.Value);
                    }
                }

                if ((!(swaggerOptions?.EnableImplicitFlow ?? false)) &&
                    (!(swaggerOptions?.EnablePasswordFlow ?? false)))
                {
                    return;
                }

                var adOptions = configuration.GetSection(ADOptions.AD).Get<ADOptions>();
                if (adOptions == null)
                {
                    throw new InvalidOperationException("Не заданы настройки AD.");
                }

                AddOAuthFlows(configuration, swaggerOptions, adOptions, options);

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }

        private static void AddOAuthFlows(IConfiguration configuration, SwaggerOptions swaggerOptions, ADOptions adOptions, SwaggerGenOptions options)
        {
            var flows = new OpenApiOAuthFlows();
            if (swaggerOptions?.EnableImplicitFlow ?? false)
            {
                if (adOptions.Authority == null || adOptions.AuthorizationUrl == null)
                {
                    throw new InvalidOperationException("Не заданы настройки AD:Authority или AD:AuthorizationUrl");
                }

                var authorizationUrl = GetConnectionString(configuration, adOptions.AuthorizationUrl) ??
                                       $"{GetConnectionString(configuration, adOptions.Authority)}/oauth2/authorize/";
                flows.Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(authorizationUrl)
                };
            }

            if (swaggerOptions?.EnablePasswordFlow ?? false)
            {
                if (adOptions.TokenUrl == null)
                {
                    throw new InvalidOperationException("Не заданы настройки AD:Authority или AD:TokenUrl");
                }

                var tokenUrl = GetConnectionString(configuration, adOptions.TokenUrl) ??
                                       $"{GetConnectionString(configuration, adOptions.Authority)}/oauth2/token/";
                flows.Password = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri(tokenUrl)
                };
            }

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = flows
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        }

        private static string GetConnectionString(IConfiguration configuration, string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentException();
            }

            if (!connectionString.Contains("${"))
            {
                return connectionString;
            }

            connectionString = connectionString.Replace("${", string.Empty);
            connectionString = connectionString.TrimEnd('}');
            connectionString = configuration.GetValue<string>(connectionString);
            return connectionString;
        }
    }
}
