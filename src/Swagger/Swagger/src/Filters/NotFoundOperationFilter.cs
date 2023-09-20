// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gems.Swagger.Filters
{
    public class NotFoundOperationFilter : IOperationFilter
    {
        private readonly Type notFoundResultType;

        public NotFoundOperationFilter(Type notFoundResultType)
        {
            this.notFoundResultType = notFoundResultType ?? throw new ArgumentNullException(nameof(notFoundResultType));
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (this.notFoundResultType == null)
            {
                return;
            }

            var status404NotFoundAsString = StatusCodes.Status404NotFound.ToString();
            if (operation.Responses.ContainsKey(status404NotFoundAsString))
            {
                return;
            }

            var response = new OpenApiResponse
            {
                Description = "Not Found",
            };

            var schema = context.SchemaGenerator.GenerateSchema(this.notFoundResultType, context.SchemaRepository);
            response.Content.Add("application/json", new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Reference = schema.Reference,
                },
            });
            operation.Responses.Add(status404NotFoundAsString, response);
        }
    }
}
