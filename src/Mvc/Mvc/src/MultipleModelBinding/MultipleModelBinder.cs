// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gems.Mvc.MultipleModelBinding
{
    public class MultipleModelBinder : IModelBinder
    {
        private readonly IModelBinder bodyBinder;
        private readonly IModelBinder complexBinder;

        public MultipleModelBinder(IModelBinder bodyBinder, IModelBinder complexBinder)
        {
            this.bodyBinder = bodyBinder;
            this.complexBinder = complexBinder;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await this.bodyBinder.BindModelAsync(bindingContext);

            if (bindingContext.Result.IsModelSet)
            {
                bindingContext.Model = bindingContext.Result.Model;
            }

            await this.complexBinder.BindModelAsync(bindingContext);
        }
    }
}
