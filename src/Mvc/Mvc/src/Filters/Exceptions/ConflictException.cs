// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Filters.Exceptions
{
    public class ConflictException : BusinessException
    {
        public ConflictException()
        {
            this.StatusCode = 409;
        }

        public ConflictException(string message) : base(message)
        {
            this.StatusCode = 409;
        }
    }
}
