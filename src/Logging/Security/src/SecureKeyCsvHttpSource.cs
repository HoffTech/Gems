// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Gems.Logging.Security
{
    public class SecureKeyCsvHttpSource : BaseSecureKeyCsvStreamSource
    {
        private readonly Uri uri;

        public SecureKeyCsvHttpSource(Uri uri, char separator = ';', Encoding encoding = null)
            : base(separator, encoding)
        {
            this.uri = uri;
        }

        protected override Stream GetStream()
        {
            using var client = new HttpClient();
            return client
                .GetStreamAsync(this.uri)
                .GetAwaiter()
                .GetResult();
        }
    }
}
