// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Caching
{
    public static class ServicesConfigurationExtension
    {
        public static void AddDistributedCache(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<DistributedCacheOptions> configureOptions = null)
        {
            var options = new DistributedCacheOptions();
            configureOptions?.Invoke(options);

            options.RedisConnectionString ??= configuration?.GetConnectionString("Redis");
            if (string.IsNullOrEmpty(options.RedisConnectionString))
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(o => o.Configuration = options.RedisConnectionString);
            }
        }
    }
}
