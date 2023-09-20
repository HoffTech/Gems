// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Text.Json;
using Gems.Utils;

using MediatR;

using Microsoft.Extensions.Caching.Distributed;

namespace Gems.Caching.Behaviors
{
    public class CacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequestCache
    {
        private readonly IDistributedCache cache;

        public CacheBehavior(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheKey = request.GetCacheKey() ?? HashUtil.CreateMd5(typeof(TRequest).FullName + request.Serialize());
            return this.cache.GetOrCreateAsync(cacheKey, () => next(), cancellationToken);
        }
    }
}
