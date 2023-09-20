// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gems.Logging.Security
{
    public abstract class BaseSecureKeyCsvStreamSource : ISecureKeySource
    {
        private readonly char separator;
        private readonly Encoding encoding;
        private Lazy<List<SecureKey>> keys;

        protected BaseSecureKeyCsvStreamSource(char separator = ';', Encoding encoding = null)
        {
            this.separator = separator;
            this.encoding = encoding ?? Encoding.UTF8;
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

        protected virtual SecureKey ParseSecureKey(string line)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                var parts = line.Split(this.separator);
                var namePattern = parts.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(namePattern))
                {
                    var valuePattern = parts.Skip(1).FirstOrDefault();
                    var replaceText = parts.Skip(2).FirstOrDefault();
                    var action = string.IsNullOrWhiteSpace(valuePattern) ? SecureKeyAction.Remove : SecureKeyAction.Update;
                    var key = new SecureKey(
                        new Regex(namePattern, RegexOptions.IgnoreCase),
                        action,
                        action == SecureKeyAction.Update ? new Regex(valuePattern, RegexOptions.IgnoreCase) : null,
                        action == SecureKeyAction.Update ? (replaceText ?? string.Empty) : null);
                    return key;
                }
            }

            return null;
        }

        protected virtual List<SecureKey> Parse(Stream sourceStream)
        {
            var keys = new List<SecureKey>();
            using var streamReader = new StreamReader(sourceStream, this.encoding);

            while (!streamReader.EndOfStream)
            {
                var key = this.ParseSecureKey(streamReader.ReadLine());
                if (key != null)
                {
                    keys.Add(key);
                }
            }

            return keys;
        }

        protected abstract Stream GetStream();

        protected Lazy<List<SecureKey>> CreateLazyKeys()
        {
            return new Lazy<List<SecureKey>>(() => this.Parse(this.GetStream()));
        }
    }
}
