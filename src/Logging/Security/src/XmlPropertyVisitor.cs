// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace Gems.Logging.Security
{
    internal class XmlPropertyVisitor : IPropertyVisitor<XElement>
    {
        public void Visit(XElement root, [AllowNull] Action<IPropertyProxy> accept)
        {
            foreach (var child in root.Attributes())
            {
                accept?.Invoke(new XAttributeProxy(child));
            }

            foreach (var child in root.Elements())
            {
                if (child.HasElements)
                {
                    this.Visit(child, accept);
                }
                else
                {
                    accept?.Invoke(new XElementProxy(child));
                }
            }
        }
    }
}
