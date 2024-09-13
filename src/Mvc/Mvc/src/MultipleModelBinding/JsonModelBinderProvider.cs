// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gems.Mvc.MultipleModelBinding
{
    public class JsonModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.Metadata.IsComplexType)
            {
                return null;
            }

            var propName = context.Metadata.PropertyName;
            var propInfo = context.Metadata.ContainerType?.GetProperty(propName);
            if (propName == null || propInfo == null)
            {
                return null;
            }

            if (context?.BindingInfo?.BindingSource?.CanAcceptDataFrom(BindingSource.Form) ?? false)
            {
                return new JsonModelBinder(context.Metadata.ModelType);
            }

            return null;
        }
    }
}
