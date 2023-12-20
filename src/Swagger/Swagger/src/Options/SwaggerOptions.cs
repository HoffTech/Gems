// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Microsoft.OpenApi.Models;

namespace Gems.Swagger.Options
{
    public class SwaggerOptions
    {
        /// <summary>
        /// Name in appsettings.json.
        /// </summary>
        public const string Swagger = "Swagger";

        public bool EnableSimpleTokenAuthorization { get; set; }

        public bool EnableImplicitFlow { get; set; }

        public bool EnablePasswordFlow { get; set; }

        /// <summary>
        /// SwaggerKey.
        /// </summary>
        public string SwaggerKey { get; set; }

        /// <summary>
        /// SwaggerName.
        /// </summary>
        public string SwaggerName { get; set; }

        /// <summary>
        /// SwaggerSchema.
        /// </summary>
        public string SwaggerSchema { get; set; }

        /// <summary>
        /// GitLabSwaggerPrefix.
        /// </summary>
        public string GitLabSwaggerPrefix { get; set; }

        /// <summary>
        /// If this option is enabled, the BadRequest response and the InternalServerError response schemes will be generated.
        /// Schemes can be overridden in a standard way - with ProducesResponseType attribute.
        /// You must also pass a validationResultType and genericErrorType into the ServiceCollectionExtensions.AddSwagger method.
        /// </summary>
        public bool EnableSchemaForErrorResponse { get; set; }

        public bool EnableAnnotations { get; set; }

        public bool UseOneOfForPolymorphism { get; set; }

        public List<string> IncludeXmlComments { get; set; }

        public Dictionary<string, OpenApiInfo> SwaggerDoc { get; set; }
    }
}
