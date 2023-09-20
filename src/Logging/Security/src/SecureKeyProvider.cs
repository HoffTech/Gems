// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace Gems.Logging.Security
{
    internal class SecureKeyProvider : ISecureKeyProvider
    {
        private readonly List<ISecureKeySource> secureKeySources;

        public SecureKeyProvider()
        {
            this.secureKeySources = new List<ISecureKeySource>();
        }

        public List<SecureKey> Keys()
        {
            return this.secureKeySources.SelectMany(x => x.Keys()).ToList();
        }

        public void Reset()
        {
            this.secureKeySources.ForEach(x => x.Reset());
        }

        public void Add(ISecureKeySource secureKeySource)
        {
            this.secureKeySources.Add(secureKeySource);
        }

        public ISecureKeySource GetSource<T>() where T : class
        {
            return this.secureKeySources.FirstOrDefault(x =>
            {
                var t = x.GetType();
                return t == typeof(T) || t.GetInterfaces().Any(i => i == typeof(T));
            });
        }
    }
}
