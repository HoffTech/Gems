// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gems.Swagger.Filters
{
    public class InternalServerErrorOperationFilter : IOperationFilter
    {
        private readonly Type genericErrorType;

        public InternalServerErrorOperationFilter(Type genericErrorType)
        {
            this.genericErrorType = genericErrorType ?? throw new ArgumentNullException(nameof(genericErrorType));
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (this.genericErrorType == null)
            {
                return;
            }

            var status500InternalServerError = StatusCodes.Status500InternalServerError.ToString();
            if (operation.Responses.ContainsKey(status500InternalServerError))
            {
                return;
            }

            var response = new OpenApiResponse
            {
                Description = "Server Error"
            };

            var schema = context.SchemaGenerator.GenerateSchema(this.genericErrorType, context.SchemaRepository);
            response.Content.Add("application/json", new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Reference = schema.Reference
                }
            });
            operation.Responses.Add(status500InternalServerError, response);
        }
    }
}
