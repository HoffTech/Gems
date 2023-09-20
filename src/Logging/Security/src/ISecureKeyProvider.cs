// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Logging.Security
{
    public interface ISecureKeyProvider
    {
        List<SecureKey> Keys();

        void Reset();

        ISecureKeySource GetSource<T>() where T : class;
    }
}
