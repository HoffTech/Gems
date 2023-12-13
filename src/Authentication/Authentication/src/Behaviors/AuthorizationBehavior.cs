// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.Filters.Exceptions;

using MediatR;

using Microsoft.AspNetCore.Http;

using InvalidOperationException = System.InvalidOperationException;

namespace Gems.Authentication.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequestAuthorization
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
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
                throw new InvalidOperationException("Не доступен HttpContext.");
            }

            if (!this.httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new AuthenticationException("Аутентификация не пройдена.");
            }

            var roles = request.GetRoles();
            if (roles == null || !roles.Any())
            {
                return next();
            }

            if (!request.GetRoles().Any(x => this.httpContextAccessor.HttpContext.User.IsInRole(x.ToString())))
            {
                throw new ForbiddenAccessException("Доступ запрещен.");
            }

            return next();
        }
    }
}
