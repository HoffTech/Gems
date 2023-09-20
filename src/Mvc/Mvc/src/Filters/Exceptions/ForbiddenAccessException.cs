// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Filters.Exceptions
{
    public class ForbiddenAccessException : BusinessException
    {
        private const int ForbiddenStatusCode = 403;

        public ForbiddenAccessException()
        {
            this.StatusCode = ForbiddenStatusCode;
        }

        public ForbiddenAccessException(string message) : base(message)
        {
            this.StatusCode = ForbiddenStatusCode;
        }

        public ForbiddenAccessException(string message, bool isBusiness) : base(message, isBusiness)
        {
            this.StatusCode = ForbiddenStatusCode;
        }
    }
}