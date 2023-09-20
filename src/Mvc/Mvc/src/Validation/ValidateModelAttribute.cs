// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc.Filters;

namespace Gems.Mvc.Validation
{
    /// <summary>
    /// Фильтр действия для выполнения валидации.
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            context.Result = new ValidationFailedResult(context.ModelState);
        }
    }
}
