// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

using FluentValidation;

using Gems.Mvc.Filters.Exceptions;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gems.Mvc.Filters.Errors
{
    public class BusinessErrorViewModel : IHasStatusCode
    {
        public BusinessErrorViewModel()
        {
        }

        public BusinessErrorViewModel(UnauthorizedAccessException exception)
        {
            this.Exception = exception;
            this.Error = new ErrorInside
            {
                IsBusiness = false,
                Message = exception.Message
            };
        }

        public BusinessErrorViewModel(ValidationException exception)
        {
            this.Exception = exception;
            this.Error = new ErrorInside
            {
                IsBusiness = false,
                Message = "Входные данные неверны.",
                Errors = exception.Errors.Select(x => x.ErrorMessage).ToArray()
            };
        }

        public BusinessErrorViewModel(ModelStateDictionary modelState)
        {
            this.ModelState = modelState;
            this.Error = new ErrorInside
            {
                Message = modelState
                .First(ms => ms.Value.Errors.Any())
                .Value.Errors.First().ErrorMessage
            };
        }

        public BusinessErrorViewModel(InvalidDataException exception)
        {
            this.Exception = exception;
            var message = GetParseCode(exception, out var code);

            this.Error = new ErrorInside
            {
                IsBusiness = true,
                Message = message,
                Code = code
            };
        }

        public BusinessErrorViewModel(System.InvalidOperationException exception, int statusCode)
        {
            this.StatusCode = statusCode;
            this.Exception = exception;
            var message = GetParseCode(exception, out var code);

            this.Error = new ErrorInside
            {
                IsBusiness = statusCode < 499,
                Message = message,
                Code = code
            };
        }

        public BusinessErrorViewModel(BusinessException exception)
        {
            this.Exception = exception;
            this.StatusCode = exception.StatusCode ?? 422;
            this.Error = exception.Error ?? new ErrorInside
            {
                Message = exception.Message
            };
            this.Error.IsBusiness ??= true;
        }

        public BusinessErrorViewModel(RequestException exception)
        {
            this.Exception = exception;
            this.Error = new ErrorInside
            {
                IsBusiness = false,
                Message = $"Произошла ошибка при выполнении внешнего запроса или запроса к бд. StatusCode: {exception.StatusCode}; {GetFullError(exception)}"
            };
        }

        public BusinessErrorViewModel(Exception exception)
        {
            this.Exception = exception;
            this.Error = new ErrorInside
            {
                Message = GetFullError(exception)
            };
        }

        [JsonIgnore]
        public string Message => this.Error?.Message;

        public ErrorInside Error { get; set; }

        [JsonIgnore]
        public Exception Exception { get; set; }

        [JsonIgnore]
        public ModelStateDictionary ModelState { get; set; }

        [JsonIgnore]
        public int? StatusCode { get; set; }

        private static string GetParseCode(Exception exception, out string code)
        {
            string message;
            code = null;
            const string codeSeparator = "[code]=";
            if (exception.Message.Contains(codeSeparator))
            {
                message = exception.Message.Split(codeSeparator)[0];
                code = exception.Message.Split(codeSeparator)[1];
            }
            else
            {
                message = exception.Message;
            }

            return message;
        }

        private static string GetFullError(Exception e)
        {
            var m = string.Empty;
            var current = e;
            while (current != null)
            {
                m += $"ExceptionType: {current.GetType().Name}, ExceptionMessage: {current.Message}, ExceptionStackTrace: {Environment.NewLine}{current.StackTrace}{Environment.NewLine}{Environment.NewLine}";
                current = current.InnerException;
            }

            return m;
        }
    }
}
