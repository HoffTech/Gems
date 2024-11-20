// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Filters.Exceptions
{
    public class InvalidOperationException : BusinessException
    {
        public InvalidOperationException()
        {
            this.StatusCode = 422;
        }

        public InvalidOperationException(string message) : base(message)
        {
            this.StatusCode = 422;
        }

        public InvalidOperationException(string message, string errorCode) : base(message, errorCode)
        {
            this.StatusCode = 422;
        }

        public InvalidOperationException(string message, string errorCode, bool isBusiness) : base(message, errorCode, isBusiness)
        {
            this.StatusCode = 422;
        }
    }
}
