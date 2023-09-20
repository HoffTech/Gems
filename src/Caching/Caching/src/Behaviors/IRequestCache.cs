// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Caching.Behaviors
{
    public interface IRequestCache
    {
        string GetCacheKey()
        {
            return null;
        }
    }
}
