// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Logging.Security
{
    public interface IPropertyFilterBuilder
    {
        IPropertyFilterBuilder Register<TElement>(IPropertyVisitor<TElement> propertyVisitor) where TElement : class;

        IPropertyFilterBuilder Register(ISecureKeySource secureKeySource);
    }
}
