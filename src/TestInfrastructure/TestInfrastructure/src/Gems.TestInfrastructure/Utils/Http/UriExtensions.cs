// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Utils.Http
{
    public static class UriExtensions
    {
        public static Uri Add(this Uri uri, string relativeUri)
        {
            var baseUri = uri.ToString();
            if (!baseUri.EndsWith("/"))
            {
                baseUri += "/";
            }

            if (relativeUri.StartsWith("/"))
            {
                relativeUri = relativeUri.Substring(1);
            }

            return new Uri(new Uri(baseUri), relativeUri);
        }
    }
}
