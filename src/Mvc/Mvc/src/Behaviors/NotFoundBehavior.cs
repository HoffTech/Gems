// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.Filters.Exceptions;

using MediatR;

namespace Gems.Mvc.Behaviors
{
    public class NotFoundBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequestNotFound
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var result = await next();
            return result == null
                ? throw new NotFoundException(request.GetNotFoundErrorMessage(), true)
                : result;
        }
    }
}
