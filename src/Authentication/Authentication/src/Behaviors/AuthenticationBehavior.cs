// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace Gems.Authentication.Behaviors
{
    public class AuthenticationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequestAuthentication
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthenticationBehavior(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (this.httpContextAccessor.HttpContext?.User.Identity == null)
            {
                throw new UnauthorizedAccessException("Не доступен HttpContext.");
            }

            if (!this.httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Аутентификация не пройдена.");
            }

            return next();
        }
    }
}
