// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gems.Mvc.MultipleModelBinding
{
    public class MultipleModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinderProvider bodyModelBinderProvider;
        private readonly IModelBinderProvider complexTypeModelBinderProvider;

        public MultipleModelBinderProvider(IModelBinderProvider bodyModelBinderProvider, IModelBinderProvider complexTypeModelBinderProvider)
        {
            this.bodyModelBinderProvider = bodyModelBinderProvider;
            this.complexTypeModelBinderProvider = complexTypeModelBinderProvider;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.BindingInfo.BindingSource != null
                && context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Body))
            {
                var bodyBinder = this.bodyModelBinderProvider.GetBinder(context);
                var complexBinder = this.complexTypeModelBinderProvider.GetBinder(context);
                return new MultipleModelBinder(bodyBinder, complexBinder);
            }
            else
            {
                return null;
            }
        }
    }
}
