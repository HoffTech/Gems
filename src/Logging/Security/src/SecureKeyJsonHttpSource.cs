// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Gems.Logging.Security
{
    public class SecureKeyJsonHttpSource : ISecureKeySource
    {
        private readonly bool throwOnError;
        private readonly Uri uri;
        private Lazy<List<SecureKey>> keys;

        public SecureKeyJsonHttpSource(Uri uri, bool throwOnError = false)
        {
            this.uri = uri;
            this.throwOnError = throwOnError;
            this.keys = this.CreateLazyKeys();
        }

        public List<SecureKey> Keys()
        {
            return this.keys.Value;
        }

        public void Reset()
        {
            this.keys = this.CreateLazyKeys();
        }

        protected Lazy<List<SecureKey>> CreateLazyKeys()
        {
            return new Lazy<List<SecureKey>>(() =>
            {
                try
                {
                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Add(
                        "Hoff-Request-Source",
                        this.GetHoffRequestSource());
                    using var stream = client
                        .GetStreamAsync(this.uri)
                        .GetAwaiter()
                        .GetResult();
                    using var reader = new StreamReader(stream);
                    using var jsonReader = new JsonTextReader(reader);
                    var serializer = new JsonSerializer();
                    return serializer
                        .Deserialize<List<SecureKeyRoot>>(jsonReader)
                        .Select(x => new SecureKey(
                            new Regex(x.Name),
                            x.Mask == null ? SecureKeyAction.Remove : SecureKeyAction.Update,
                            x.Mask != null ? new Regex(x.Mask.Search) : null,
                            x.Mask?.Replace))
                        .ToList();
                }
                catch
                {
                    if (this.throwOnError)
                    {
                        throw;
                    }
                    else
                    {
                        return new List<SecureKey>();
                    }
                }
            });
        }

        private string GetHoffRequestSource()
        {
            var name = Assembly.GetEntryAssembly().GetName();
            return $"{name.Name} {name.Version}";
        }

        protected class SecureKeyMask
        {
            [JsonProperty("search")]
            public string Search { get; set; }

            [JsonProperty("replace")]
            public string Replace { get; set; }
        }

        protected class SecureKeyRoot
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("mask")]
            public SecureKeyMask Mask { get; set; }
        }
    }
}
