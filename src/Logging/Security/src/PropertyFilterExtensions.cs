// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

namespace Gems.Logging.Security
{
    public static class PropertyFilterExtensions
    {
        public static string FilterJson(this IPropertyFilter<JToken> filter, string json)
        {
            var token = JToken.Parse(json);
            filter.Filter(token);
            return token.ToString();
        }

        public static string FilterXml(this IPropertyFilter<XElement> filter, string xml)
        {
            var x = XDocument.Parse(xml);
            filter.Filter(x.Root);
            return x.ToString();
        }

        public static TObject FilterObject<TObject>(this IPropertyFilter<ObjectToJsonProjection> filter, TObject obj)
            where TObject : class
        {
            var source = filter.SecureKeyProvider.GetSource<IObjectPropertyFilterSource>() as IObjectPropertyFilterSource;
            source?.Register(obj.GetType());
            var token = JToken.FromObject(obj);
            var projection = new ObjectToJsonProjection(obj, token);
            filter.Filter(projection);
            return token.ToObject<TObject>();
        }
    }
}
