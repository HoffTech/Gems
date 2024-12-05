// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Gems.Text.Json;

using Microsoft.Extensions.Logging;

namespace Gems.Logging.Mvc.LogsCollector
{
    public class RequestLogsCollector
    {
        private const string ResponseStatusKey = "responseStatus";

        private readonly ILogger logger;
        private readonly List<List<LogLevelOptions>> logLevelsByHttpStatus;

        private readonly Dictionary<string, object> logs = new Dictionary<string, object>();

        public RequestLogsCollector(ILogger logger)
        {
            this.logger = logger;
            this.logLevelsByHttpStatus = new List<List<LogLevelOptions>>
                                         {
                                             RequestLogsCollectorOptions.DefaultLogLevelsByHttpStatus
                                         };
        }

        public RequestLogsCollector(ILogger logger, List<List<LogLevelOptions>> logLevelsByHttpStatus = null)
        {
            this.logger = logger;
            this.logLevelsByHttpStatus = logLevelsByHttpStatus?.Where(x => x != null).ToList()
                                         ?? new List<List<LogLevelOptions>>
                                         {
                                             RequestLogsCollectorOptions.DefaultLogLevelsByHttpStatus
                                         };
        }

        public void AddLogsFromPayload(object obj)
        {
            if (obj == null)
            {
                return;
            }

            var objType = obj.GetType();
            if (objType.IsValueType || obj is string)
            {
                return;
            }

            if (obj is IEnumerable objAsList)
            {
                foreach (var child in objAsList)
                {
                    this.AddLogsFromPayload(child);
                }

                return;
            }

            var properties = objType.GetProperties().Where(NeedToLog);
            foreach (var property in properties)
            {
                var logFieldAttribute = (LogFieldAttribute)Attribute.GetCustomAttribute(property, typeof(LogFieldAttribute));
                var logFieldName = $"{Assembly.GetEntryAssembly()?.GetName().Name}_{logFieldAttribute?.Name ?? property.Name}";
                var propValue = property.GetValue(obj, null);
                if (property.PropertyType.IsValueType || propValue is string)
                {
                    this.AddOrUpdateValueInLogs(logFieldName, propValue);
                }
                else if (propValue is IEnumerable list)
                {
                    foreach (var child in list)
                    {
                        this.AddLogsFromPayload(child);
                    }
                }
                else
                {
                    this.AddLogsFromPayload(propValue);
                }
            }
        }

        public void AddRequestHeaders(Dictionary<string, string> headers)
        {
            headers.Remove("Authorization");
            this.AddOrUpdateValueInLogs("requestHeaders", headers);
        }

        public void AddResponseHeaders(Dictionary<string, string> headers)
        {
            this.AddOrUpdateValueInLogs("responseHeaders", headers);
        }

        public virtual void AddRequest(object data)
        {
            var dataAsString = MapDataToString(data);
            this.AddOrUpdateValueInLogs("requestBody", dataAsString);
        }

        public virtual void AddResponse(object data)
        {
            var dataAsString = MapDataToString(data);
            this.AddOrUpdateValueInLogs("responseBody", dataAsString);
        }

        public void AddStatus(int status)
        {
            this.AddOrUpdateValueInLogs(ResponseStatusKey, status);
        }

        public void AddPath(string path)
        {
            this.AddOrUpdateValueInLogs("requestPath", path);
        }

        public void AddEndpointSummary(string endpointSummary)
        {
            this.AddOrUpdateValueInLogs("endpointSummary", endpointSummary);
        }

        public void AddValue(string key, object value)
        {
            this.AddOrUpdateValueInLogs(key, value);
        }

        public void AddRequestDuration(double durationMilliseconds)
        {
            this.AddOrUpdateValueInLogs("requestDuration", durationMilliseconds);
        }

        public void WriteLogs()
        {
            var template = string.Join(", ", this.logs.Keys.Select(x => $"{x}: {{{x}}}"));
            var args = this.logs.Values.ToArray();

            this.logger?.Log(this.GetLogLevelByHttpStatus(), template, args);
        }

        private static string MapDataToString(object data)
        {
            if (data == null)
            {
                return null;
            }

            if (data.GetType().IsValueType)
            {
                return data.ToString();
            }

            if (data is string dataAsString)
            {
                return dataAsString;
            }

            return data.Serialize(camelCase: true);
        }

        private static bool NeedToLog(MemberInfo property)
        {
            return property.CustomAttributes.Any(x => x.AttributeType == typeof(LogFieldAttribute));
        }

        private void AddOrUpdateValueInLogs(string key, object value)
        {
            if (this.logs.ContainsKey(key))
            {
                this.logs[key] = value;
            }
            else
            {
                this.logs.Add(key, value);
            }
        }

        private LogLevel GetLogLevelByHttpStatus()
        {
            if (this.logs.TryGetValue(ResponseStatusKey, out var responseStatus))
            {
                if (responseStatus is int intResponseStatus)
                {
                    if (this.TryGetLogLevelByHttpStatus(intResponseStatus, out var logLevel))
                    {
                        return logLevel;
                    }

                    if (intResponseStatus <= 399 && this.TryGetLogLevelByHttpStatus(200, out logLevel))
                    {
                        return logLevel;
                    }

                    if (intResponseStatus <= 499 && this.TryGetLogLevelByHttpStatus(400, out logLevel))
                    {
                        return logLevel;
                    }

                    if (intResponseStatus <= 599 && this.TryGetLogLevelByHttpStatus(500, out logLevel))
                    {
                        return logLevel;
                    }

                    if (intResponseStatus is >= 400 and <= 599)
                    {
                        return LogLevel.Error;
                    }
                }
            }

            return LogLevel.Information;
        }

        private bool TryGetLogLevelByHttpStatus(int status, out LogLevel level)
        {
            foreach (var logLevelsByHttpStatusItem in this.logLevelsByHttpStatus)
            {
                foreach (var logLevelsByHttpStatusItemInner in logLevelsByHttpStatusItem)
                {
                    if (logLevelsByHttpStatusItemInner.Status == status)
                    {
                        level = logLevelsByHttpStatusItemInner.Level;
                        return true;
                    }
                }
            }

            level = LogLevel.None;
            return false;
        }
    }
}
