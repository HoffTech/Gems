// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Logging.Security
{
    public interface IPropertyFilter<TElement> where TElement : class
    {
        ISecureKeyProvider SecureKeyProvider { get; }

        void Filter(TElement root);
    }
}
