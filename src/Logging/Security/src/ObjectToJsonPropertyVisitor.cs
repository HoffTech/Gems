// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

namespace Gems.Logging.Security
{
    internal class ObjectToJsonPropertyVisitor : IPropertyVisitor<ObjectToJsonProjection>
    {
        public void Visit(ObjectToJsonProjection root, [AllowNull] Action<IPropertyProxy> accept)
        {
            foreach (var child in root.Token.Children())
            {
                if (child is JProperty prop)
                {
                    var propValue = prop.Value;
                    if (propValue != null && propValue is JObject objPropValueToken)
                    {
                        var subObject = root.ObjectType.GetProperty(prop.Name);
                        if (subObject != null)
                        {
                            var subObjectValue = subObject.GetValue(root.Object, null);
                            this.Visit(new ObjectToJsonProjection(subObjectValue, objPropValueToken), accept);
                        }
                    }
                    else if (propValue != null && propValue is JArray arrPropValue)
                    {
                        var subObject = root.ObjectType.GetProperty(prop.Name);
                        if (subObject != null)
                        {
                            var subObjectValue = subObject.GetValue(root.Object, null);
                            if (subObjectValue is IList list)
                            {
                                for (var i = 0; i < list.Count; i++)
                                {
                                    var objItem = list[i];
                                    var tokenItem = arrPropValue[i];
                                    this.Visit(new ObjectToJsonProjection(objItem, tokenItem), accept);
                                }
                            }
                        }
                    }
                    else
                    {
                        accept?.Invoke(new JsonTypedPropertyProxy(prop, root.ObjectType));
                    }
                }
            }
        }
    }
}
