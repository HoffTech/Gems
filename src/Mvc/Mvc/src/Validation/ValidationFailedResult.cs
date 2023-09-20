// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gems.Mvc.Validation
{
    /// <summary>
    /// Неуспешный результат валидации.
    /// </summary>
    public class ValidationFailedResult : ObjectResult
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ValidationFailedResult"/>.
        /// </summary>
        /// <param name="modelState">модель результата валидации.</param>
        public ValidationFailedResult(ModelStateDictionary modelState)
            : base(new ValidationResultModel(modelState))
        {
            this.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
