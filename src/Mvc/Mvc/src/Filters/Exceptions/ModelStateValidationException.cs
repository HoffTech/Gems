// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gems.Mvc.Filters.Exceptions
{
    public class ModelStateValidationException : Exception
    {
        public ModelStateValidationException(ModelStateDictionary modelState)
        {
            this.ModelState = modelState;
        }

        public ModelStateDictionary ModelState { get; }
    }
}
