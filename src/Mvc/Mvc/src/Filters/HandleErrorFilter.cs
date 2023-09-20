// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Mvc.Filters.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gems.Mvc.Filters
{
    public class HandleErrorFilter : ExceptionFilterAttribute
    {
        private readonly IConverter<Exception, BusinessErrorViewModel> exceptionToModelConverter;

        public HandleErrorFilter(IConverter<Exception, BusinessErrorViewModel> exceptionToModelConverter)
        {
            this.exceptionToModelConverter = exceptionToModelConverter;
        }

        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            var model = this.exceptionToModelConverter.Convert(context.Exception);
            context.Result = new ObjectResult(this.MapErrorModel(model))
            {
                StatusCode = model.StatusCode ?? 499
            };
            context.ExceptionHandled = true;
        }

        protected virtual object MapErrorModel(BusinessErrorViewModel model)
        {
            return model;
        }
    }
}
