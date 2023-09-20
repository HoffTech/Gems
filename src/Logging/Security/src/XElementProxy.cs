// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Xml.Linq;

namespace Gems.Logging.Security
{
    internal class XElementProxy : IPropertyProxy
    {
        public XElementProxy(XElement element)
        {
            this.Element = element;
        }

        public XElement Element { get; set; }

        public object Value => this.Element.Value;

        public string Name => this.Element.Name.LocalName;

        public Type RootType => null;

        public void Remove()
        {
            this.Element.Remove();
        }

        public void Update(string value)
        {
            this.Element.SetValue(value);
        }
    }
}
