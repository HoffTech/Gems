// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Threading.Tasks;

using Gems.Mvc.Filters.Exceptions;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Gems.Mvc.Filters
{
    public class ModelStateValidationFilter : IAsyncActionFilter
    {
        private readonly DelegateConverterProvider<ModelStateValidationException, Exception> delegateConverterProvider;

        public ModelStateValidationFilter(DelegateConverterProvider<ModelStateValidationException, Exception> delegateConverterProvider)
        {
            this.delegateConverterProvider = delegateConverterProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var exception = new ModelStateValidationException(context.ModelState);
                if (context.ActionArguments.Count == 0)
                {
                    throw exception;
                }

                var delegateConverter = this.delegateConverterProvider.GetConverter(context.ActionArguments.First().Value);
                if (delegateConverter != null)
                {
                    throw delegateConverter.Convert(exception);
                }

                throw exception;
            }

            await next().ConfigureAwait(false);
        }
    }
}
