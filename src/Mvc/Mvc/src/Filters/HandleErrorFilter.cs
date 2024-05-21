// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;

using Gems.Mvc.Filters.Errors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gems.Mvc.Filters
{
    public class HandleErrorFilter : ExceptionFilterAttribute
    {
        private readonly IConverter<Exception, BusinessErrorViewModel> exceptionToModelConverter;
        private readonly DelegateConverterProvider<BusinessErrorViewModel, object> delegateConverterProvider;

        public HandleErrorFilter(
            IConverter<Exception, BusinessErrorViewModel> exceptionToModelConverter,
            DelegateConverterProvider<BusinessErrorViewModel, object> delegateConverterProvider)
        {
            this.exceptionToModelConverter = exceptionToModelConverter;
            this.delegateConverterProvider = delegateConverterProvider;
        }

        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            var model = this.exceptionToModelConverter.Convert(context.Exception);
            context.Result = new ObjectResult(this.MapErrorModel(model, context))
            {
                StatusCode = model.StatusCode ?? 499
            };
            context.ExceptionHandled = true;
        }

        protected virtual object MapErrorModel(BusinessErrorViewModel model, ExceptionContext context)
        {
            if (context.ActionDescriptor.Parameters.Count == 0)
            {
                return model;
            }

            var delegateConverter = this.delegateConverterProvider.GetConverter(context.ActionDescriptor.Parameters.First().ParameterType);
            if (delegateConverter == null)
            {
                return model;
            }

            return delegateConverter.Convert(model);
        }
    }
}
