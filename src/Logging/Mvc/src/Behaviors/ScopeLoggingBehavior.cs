// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Gems.Logging.Mvc.Behaviors
{
    public class ScopeLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequestScopeLogging
    {
        private readonly ILogger<ScopeLoggingBehavior<TRequest, TResponse>> logger;

        public ScopeLoggingBehavior(ILogger<ScopeLoggingBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            using (this.logger.BeginScope(
                       new[]
                       {
                           new KeyValuePair<string, object>("Scope", request.GetScopeId() ?? typeof(TRequest).Name)
                       }))
            {
                return await next().ConfigureAwait(false);
            }
        }
    }
}
