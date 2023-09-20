// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Net;

namespace Gems.Http
{
    public class RequestException<TError> : Gems.Mvc.Filters.Exceptions.RequestException<TError>
    {
        public RequestException(string message, TError error, HttpStatusCode statusCode) : base(message, error, statusCode)
        {
        }

        public RequestException(string message, TError error, HttpStatusCode statusCode, string body) : base(message, error, statusCode, body)
        {
        }

        public RequestException(string message, HttpStatusCode statusCode) : base(message, statusCode)
        {
        }

        public RequestException(string message, Exception exception, HttpStatusCode statusCode) : base(message, exception, statusCode)
        {
        }
    }
}
