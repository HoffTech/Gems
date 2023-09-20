// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Filters.Exceptions
{
    public class NotFoundException : BusinessException
    {
        public NotFoundException()
        {
            this.StatusCode = 404;
        }

        public NotFoundException(string message) : base(message)
        {
            this.StatusCode = 404;
        }

        public NotFoundException(string message, bool isBusiness) : base(message, isBusiness)
        {
            this.StatusCode = 404;
        }
    }
}
