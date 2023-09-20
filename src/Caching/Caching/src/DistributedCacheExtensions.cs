// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using Newtonsoft.Json;

namespace Gems.Caching
{
    public static class DistributedCacheExtensions
    {
        private static readonly DistributedCacheEntryOptions Options;

        private static readonly JsonSerializerSettings Settings;

        static DistributedCacheExtensions()
        {
            Settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            Options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
        }

        public static TResult GetOrCreate<TResult>(this IDistributedCache cache, string key, DistributedCacheEntryOptions cacheOptions, Func<TResult> func)
        {
            string value;
            TResult obj;
            try
            {
                value = cache.GetString(key);
                if (value != null)
                {
                    obj = JsonConvert.DeserializeObject<TResult>(value, Settings);
                    return obj;
                }
            }
            catch
            {
                // ignored
            }

            obj = func();
            try
            {
                value = JsonConvert.SerializeObject(obj, Settings);
                cache.SetString(key, value, cacheOptions);
            }
            catch
            {
                // ignored
            }

            return obj;
        }

        public static TResult GetOrCreate<TResult>(this IDistributedCache cache, string key, Func<TResult> func)
        {
            return cache.GetOrCreate(key, Options, func);
        }

        public static async Task<TResult> GetOrCreateAsync<TResult>(this IDistributedCache cache, string key, DistributedCacheEntryOptions cacheOptions, Func<Task<TResult>> func, CancellationToken cancellationToken)
        {
            string value;
            TResult obj;
            try
            {
                value = await cache.GetStringAsync(key, cancellationToken);
                if (value != null)
                {
                    obj = JsonConvert.DeserializeObject<TResult>(value, Settings);
                    return obj;
                }
            }
            catch
            {
                // ignored
            }

            obj = await func();
            try
            {
                value = JsonConvert.SerializeObject(obj, Settings);
                await cache.SetStringAsync(key, value, cacheOptions, cancellationToken);
            }
            catch
            {
                // ignored
            }

            return obj;
        }

        public static Task<TResult> GetOrCreateAsync<TResult>(this IDistributedCache cache, string key, Func<Task<TResult>> func, CancellationToken cancellationToken)
        {
            return cache.GetOrCreateAsync(key, Options, func, cancellationToken);
        }
    }
}
