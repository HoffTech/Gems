// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gems.Swagger.Filters
{
    public class DescriptionOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (ControllerRegister.ControllerInfos.TryGetValue(context.MethodInfo.DeclaringType, out var endpoint) && endpoint.Summary != null)
            {
                operation.Summary = endpoint.Summary;
            }
        }
    }
}
