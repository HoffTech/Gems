// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Filters.Errors
{
    public class ValidationErrorViewModel
    {
        public ValidationErrorViewModel(string field, string message)
        {
            this.Field = field != string.Empty ? field : null;
            this.Message = message;
        }

        public string Field { get; }

        public string Message { get; }
    }
}
