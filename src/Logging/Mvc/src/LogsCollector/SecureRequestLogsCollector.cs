// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Linq;
using System.Xml.Linq;

using Gems.Logging.Security;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

namespace Gems.Logging.Mvc.LogsCollector
{
    public class SecureRequestLogsCollector : RequestLogsCollector
    {
        private readonly IPropertyFilter<ObjectToJsonProjection> objectFilter;
        private readonly IPropertyFilter<JToken> jsonFilter;
        private readonly IPropertyFilter<XElement> xmlFilter;

        public SecureRequestLogsCollector(
            IPropertyFilter<ObjectToJsonProjection> objectFilter,
            IPropertyFilter<JToken> jsonFilter,
            IPropertyFilter<XElement> xmlFilter,
            ILogger logger)
            : base(logger)
        {
            this.objectFilter = objectFilter;
            this.jsonFilter = jsonFilter;
            this.xmlFilter = xmlFilter;
        }

        public override void AddRequest(object data)
        {
            data = this.FilterObject(data);
            base.AddRequest(data);
        }

        public override void AddResponse(object data)
        {
            data = this.FilterObject(data);
            base.AddResponse(data);
        }

        private object FilterObject(object data)
        {
            try
            {
                if (data == null)
                {
                    return data;
                }

                if (data.GetType().IsValueType)
                {
                    return data;
                }

                if (data is string dataAsString)
                {
                    switch (dataAsString?.FirstOrDefault())
                    {
                        case '{':
                        case '[':
                            dataAsString = this.jsonFilter.FilterJson(dataAsString);
                            break;
                        case '<':
                            dataAsString = this.xmlFilter.FilterXml(dataAsString);
                            break;
                        default:
                            break;
                    }

                    return dataAsString;
                }

                if (!(this.objectFilter.SecureKeyProvider.GetSource<IObjectPropertyFilterSource>() is
                        IObjectPropertyFilterSource source))
                {
                    return data;
                }

                source.Register(data.GetType());
                var token = JToken.FromObject(data);
                var root = new ObjectToJsonProjection(data, token);
                this.objectFilter.Filter(root);
                return token.ToObject(data.GetType());
            }
            catch
            {
                return data;
            }
        }
    }
}
