// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml.Linq;

using Gems.Logging.Security;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

namespace Gems.Logging.Mvc.LogsCollector
{
    public class SecureRequestLogsCollectorFactory : IRequestLogsCollectorFactory
    {
        private readonly IPropertyFilter<ObjectToJsonProjection> objectFilter;
        private readonly IPropertyFilter<JToken> jsonFilter;
        private readonly IPropertyFilter<XElement> xmlFilter;
        private readonly ILogger logger;

        public SecureRequestLogsCollectorFactory(
            IPropertyFilter<ObjectToJsonProjection> objectFilter,
            IPropertyFilter<JToken> jsonFilter,
            IPropertyFilter<XElement> xmlFilter,
            ILogger<SecureRequestLogsCollector> logger)
        {
            this.objectFilter = objectFilter;
            this.jsonFilter = jsonFilter;
            this.xmlFilter = xmlFilter;
            this.logger = logger;
        }

        public RequestLogsCollector Create()
        {
            return this.Create(null, null);
        }

        public RequestLogsCollector Create(ILogger loggerInstance)
        {
            return this.Create(loggerInstance, null);
        }

        public RequestLogsCollector Create(ILogger loggerInstance, List<List<LogLevelOptions>> logLevelsByHttpStatus)
        {
            return new SecureRequestLogsCollector(this.objectFilter, this.jsonFilter, this.xmlFilter, loggerInstance ?? this.logger, logLevelsByHttpStatus);
        }
    }
}
