// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.Filters.Exceptions;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace Gems.Mvc.Behaviors
{
    public class AddRetryAfterHeaderBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequestAddRetryAfterHeader
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AddRetryAfterHeaderBehavior(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next().ConfigureAwait(false);
            }
            catch (TooManyRequestsException)
            {
                this.httpContextAccessor
                    .HttpContext!
                    .Response
                    .Headers
                    .Append("retry-after", request.GetRetryAfterInterval().ToString());

                throw;
            }
        }
    }
}
