// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Mvc.Filters.Errors;

namespace Gems.Mvc.Filters.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException()
        {
            this.Error = new ErrorInside();
        }

        public BusinessException(BusinessErrorViewModel errorModel)
        {
            this.Error = errorModel?.Error;
            this.StatusCode = errorModel?.StatusCode;
        }

        public BusinessException(RequestException<BusinessErrorViewModel> exception, int? statusCode = null)
        {
            this.Error = exception?.Error?.Error;
            this.StatusCode = statusCode ?? 422;
        }

        public BusinessException(string message)
        {
            this.Error = new ErrorInside
            {
                Message = message
            };
        }

        public BusinessException(string message, int statusCode)
        {
            this.Error = new ErrorInside
            {
                Message = message
            };
            this.StatusCode = statusCode;
        }

        public BusinessException(string message, string errorCode)
        {
            this.Error = new ErrorInside
            {
                Message = message,
                Code = errorCode
            };
        }

        public BusinessException(string message, string errorCode, bool isBusiness)
        {
            this.Error = new ErrorInside
            {
                Message = message,
                Code = errorCode,
                IsBusiness = isBusiness
            };
        }

        public BusinessException(string message, bool isBusiness)
        {
            this.Error = new ErrorInside
            {
                Message = message,
                IsBusiness = isBusiness
            };
        }

        public override string Message => this.Error?.Message;

        public ErrorInside Error { get; set; }

        public int? StatusCode { get; set; }
    }
}
