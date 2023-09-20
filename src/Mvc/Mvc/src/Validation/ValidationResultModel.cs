// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gems.Mvc.Validation
{
    /// <summary>
    /// Модель результата валидации.
    /// </summary>
    public class ValidationResultModel
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ValidationResultModel"/>.
        /// </summary>
        /// <param name="modelState">состояние модели.</param>
        public ValidationResultModel(ModelStateDictionary modelState)
        {
            this.Message = "Validation Failed";

            this.Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                    .ToList();
        }

        /// <summary>
        /// Сообщение.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Список сообщений.
        /// </summary>
        public List<ValidationError> Errors { get; }
    }
}
