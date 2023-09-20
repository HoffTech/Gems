// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Net;

namespace Gems.Http
{
    public class DeserializeException : Exception
    {
        public DeserializeException(Exception exception, string responseAsString, HttpStatusCode statusCode) : base("Failed deserialization.", exception)
        {
            this.ResponseAsString = responseAsString;
            this.StatusCode = statusCode;
        }

        public string ResponseAsString { get; }

        public HttpStatusCode StatusCode { get; }
    }
}
