// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading.Tasks;

using Gems.Mvc.Filters.Exceptions;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Gems.Mvc.Filters
{
    public class ModelStateValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                throw new ModelStateValidationException(context.ModelState);
            }

            await next().ConfigureAwait(false);
        }
    }
}
