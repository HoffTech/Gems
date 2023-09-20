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
    public class ClearCacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequestClearCache
    {
        private readonly IDistributedCache cache;

        public ClearCacheBehavior(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var result = await next().ConfigureAwait(false);
            var cacheKey = request.GetCacheKey() ?? HashUtil.CreateMd5(typeof(TRequest).FullName + request.Serialize());
            await this.cache.RemoveAsync(cacheKey, cancellationToken);
            return result;
        }
    }
}
