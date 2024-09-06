// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gems.Mvc.MultipleModelBinding
{
    public class JsonModelBinder : IModelBinder
    {
        private readonly Type targetType;

        public JsonModelBinder(Type type)
        {
            this.targetType = type ?? throw new ArgumentNullException(nameof(type));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var valueAsString = valueProviderResult.FirstValue;
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject(valueAsString, this.targetType);
            if (result != null)
            {
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
