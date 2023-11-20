// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
#if NET6_0
using System.Runtime.Serialization;
#endif

namespace Gems.IO.SmbStorage
{
    public class SmbException : Exception
    {
        public SmbException()
        {
        }

        public SmbException(string message) : base(message)
        {
        }

        public SmbException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if NET6_0
        protected SmbException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}
