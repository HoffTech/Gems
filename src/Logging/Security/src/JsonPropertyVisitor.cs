// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

namespace Gems.Logging.Security
{
    internal class JsonPropertyVisitor : IPropertyVisitor<JToken>
    {
        public void Visit(JToken root, [AllowNull] Action<IPropertyProxy> accept)
        {
            foreach (var child in root.Children())
            {
                if (child is JProperty prop)
                {
                    var propValue = prop.Value;
                    if (propValue != null && propValue is JObject objPropValue)
                    {
                        this.Visit(objPropValue, accept);
                    }
                    else if (propValue != null && propValue is JArray arrPropValue)
                    {
                        this.Visit(arrPropValue, accept);
                    }
                    else
                    {
                        accept?.Invoke(new JsonPropertyProxy(prop));
                    }
                }
                else if (child.Parent is JArray && child is JValue valChild)
                {
                    accept?.Invoke(new JsonArrayElementProxy(valChild));
                }
                else if (child.Parent is JArray && child is JObject objChild)
                {
                    this.Visit(objChild, accept);
                }
            }
        }
    }
}
