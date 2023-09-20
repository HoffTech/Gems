// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Net;

using Gems.Text.Json;

namespace Gems.Mvc.Filters.Exceptions
{
    public class RequestException : Exception
    {
        public RequestException(string message, Exception innerException, HttpStatusCode statusCode)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCode? StatusCode { get; set; }
    }

    public class RequestException<TError> : RequestException
    {
        public RequestException(string message, TError error, HttpStatusCode statusCode)
            : this(message, error, statusCode, null)
        {
        }

        public RequestException(string message, TError error, HttpStatusCode statusCode, string body)
            : base($"{message} InnerResponse:{error.Serialize()}", null, statusCode)
        {
            this.Error = error;
            this.StatusCode = statusCode;
            this.Body = body;
        }

        public RequestException(string message, HttpStatusCode statusCode)
            : this(message, null, statusCode)
        {
        }

        public RequestException(string message, Exception exception, HttpStatusCode statusCode)
            : base(message, exception, statusCode)
        {
            var type = typeof(TError);
            if (type == typeof(string))
            {
                this.Error = (TError)GetFullError(exception);
            }
            else if (CheckConstructorWithExceptionParameter(type))
            {
                this.Error = (TError)Activator.CreateInstance(type, exception ?? this);
            }
            else
            {
                this.Error = default;
            }

            this.StatusCode = statusCode;
        }

        public TError Error { get; }

        public string Body { get; set; }

        private static object GetFullError(Exception e)
        {
            var m = string.Empty;
            var current = e;
            while (current != null)
            {
                m += $"ExceptionType: {current.GetType().Name}, ExceptionMessage: {current.Message}, ExceptionStackTrace: {Environment.NewLine}{current.StackTrace}{Environment.NewLine}";
                current = current.InnerException;
            }

            return m;
        }

        private static bool CheckConstructorWithExceptionParameter(Type type)
        {
            return type.GetConstructors().Any(t => t.GetParameters().Count() == 1 && t.GetParameters().First().ParameterType == typeof(Exception));
        }
    }
}
