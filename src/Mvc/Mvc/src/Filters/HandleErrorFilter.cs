// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Mvc.Filters.Errors;
using Gems.Mvc.GenericControllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Gems.Mvc.Filters
{
    public class HandleErrorFilter : ExceptionFilterAttribute
    {
        private readonly IConverter<Exception, BusinessErrorViewModel> exceptionToModelConverter;
        private readonly IOptions<ExceptionHandlingOptions> options;

        public HandleErrorFilter(
            IConverter<Exception,
            BusinessErrorViewModel> exceptionToModelConverter,
            IOptions<ExceptionHandlingOptions> options)
        {
            this.exceptionToModelConverter = exceptionToModelConverter;
            this.options = options;
        }

        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            var controllerType = context.HttpContext?.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>()?.MethodInfo.DeclaringType;
            if (this.options.Value.UseHandleErrorFilterOnNonGenericControllers ||
                (controllerType != null && ControllerRegister.ControllerInfos.TryGetValue(controllerType, out _)))
            {
                var model = this.exceptionToModelConverter.Convert(context.Exception);
                context.Result = new ObjectResult(this.MapErrorModel(model))
                {
                    StatusCode = model.StatusCode ?? 499
                };

                context.ExceptionHandled = true;
            }
        }

        protected virtual object MapErrorModel(BusinessErrorViewModel model)
        {
            return model;
        }
    }
}
