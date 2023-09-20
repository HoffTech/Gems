// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.IO;
using System.Text;

namespace Gems.Logging.Security
{
    public class SecureKeyCsvFileSource : BaseSecureKeyCsvStreamSource
    {
        private readonly string fileName;

        public SecureKeyCsvFileSource(string fileName, char separator = ';', Encoding encoding = null)
            : base(separator, encoding)
        {
            this.fileName = fileName;
        }

        protected override Stream GetStream()
        {
            return File.OpenRead(this.fileName);
        }
    }
}
