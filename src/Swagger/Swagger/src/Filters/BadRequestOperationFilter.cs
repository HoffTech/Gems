// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gems.Swagger.Filters
{
    public class BadRequestOperationFilter : IOperationFilter
    {
        private readonly Type validationResultType;

        public BadRequestOperationFilter(Type validationResultType)
        {
            this.validationResultType = validationResultType ?? throw new ArgumentNullException(nameof(validationResultType));
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (this.validationResultType == null)
            {
                return;
            }

            var status400BadRequestAsString = StatusCodes.Status400BadRequest.ToString();
            if (operation.Responses.ContainsKey(status400BadRequestAsString))
            {
                return;
            }

            var response = new OpenApiResponse
            {
                Description = "Bad Request",
            };

            var schema = context.SchemaGenerator.GenerateSchema(this.validationResultType, context.SchemaRepository);
            response.Content.Add("application/json", new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Reference = schema.Reference,
                },
            });
            operation.Responses.Add(status400BadRequestAsString, response);
        }
    }
}
