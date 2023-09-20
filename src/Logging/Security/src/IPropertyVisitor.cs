// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Gems.Logging.Security
{
    public interface IPropertyVisitor<TElement>
        where TElement : class
    {
        void Visit(TElement root, [AllowNull] Action<IPropertyProxy> accept);
    }
}
