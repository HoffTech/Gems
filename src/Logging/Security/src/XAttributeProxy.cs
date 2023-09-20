// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Xml.Linq;

namespace Gems.Logging.Security
{
    internal class XAttributeProxy : IPropertyProxy
    {
        public XAttributeProxy(XAttribute attribute)
        {
            this.Attribute = attribute;
        }

        public XAttribute Attribute { get; set; }

        public object Value => this.Attribute.Value;

        public string Name => this.Attribute.Name.LocalName;

        public Type RootType => null;

        public void Remove()
        {
            this.Attribute.Remove();
        }

        public void Update(string value)
        {
            this.Attribute.SetValue(value);
        }
    }
}
