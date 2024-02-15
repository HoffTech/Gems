// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Filters.Exceptions
{
    public class InvalidDataException : BusinessException
    {
        private const int InvalidDataStatusCode = 400;

        public InvalidDataException()
        {
            this.StatusCode = InvalidDataStatusCode;
        }

        public InvalidDataException(string message) : base(message)
        {
            this.StatusCode = InvalidDataStatusCode;
        }

        public InvalidDataException(string message, bool isBusiness) : base(message, isBusiness)
        {
            this.StatusCode = InvalidDataStatusCode;
        }
    }
}
