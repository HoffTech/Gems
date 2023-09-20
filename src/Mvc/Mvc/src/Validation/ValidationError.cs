// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Validation
{
    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ValidationError"/>.
        /// </summary>
        /// <param name="field">поле для которого сработала ошибка.</param>
        /// <param name="message">сообщение об ошибке.</param>
        public ValidationError(string field, string message)
        {
            this.Field = field != string.Empty ? field : null;
            this.Message = message;
        }

        /// <summary>
        /// Поле для которго сработала ошибка.
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        public string Message { get; }
    }
}
