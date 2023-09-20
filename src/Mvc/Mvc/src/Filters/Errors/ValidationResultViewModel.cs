// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

using FluentValidation;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gems.Mvc.Filters.Errors
{
    public class ValidationResultViewModel : ErrorViewModel
    {
        public ValidationResultViewModel() : this((ModelStateDictionary)null) { }

        public ValidationResultViewModel(ValidationException exception)
        {
            this.Message = exception.Message;
            if (exception.Errors != null)
            {
                this.Errors = exception.Errors
                    .Select(a => new ValidationErrorViewModel(a.PropertyName, a.ErrorMessage)).ToList();
            }
        }

        public ValidationResultViewModel(ModelStateDictionary modelState)
        {
            this.Message = @"Validation Failed";
            if (modelState == null)
            {
                return;
            }

            this.Errors = new List<ValidationErrorViewModel>();
            modelState.Keys.ToList().ForEach(key =>
            {
                var messages = modelState[key].Errors.ToList().Select(x =>
                {
                    var message = x.ErrorMessage;
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        return message;
                    }

                    message = x.Exception is Newtonsoft.Json.JsonException ? $"The data in {key} field is of incorrect format" : x.Exception.Message;

                    return message;
                });
                if (!messages.Any() && modelState[key].ValidationState == ModelValidationState.Unvalidated)
                {
                    messages = new string[] { $"The data in {key} field is of incorrect format" };
                }

                messages.Distinct().ToList().ForEach(m => this.Errors.Add(new ValidationErrorViewModel(key, m)));
            });
        }

        public List<ValidationErrorViewModel> Errors { get; }
    }
}
