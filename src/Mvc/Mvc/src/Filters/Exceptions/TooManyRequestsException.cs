// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Filters.Exceptions
{
    public class TooManyRequestsException : BusinessException
    {
        public TooManyRequestsException()
        {
            this.StatusCode = 429;
        }

        public TooManyRequestsException(string message) : base(message)
        {
            this.StatusCode = 429;
        }
    }
}
