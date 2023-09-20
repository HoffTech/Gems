// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Gems.Logging.Security;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gems.Logging.Mvc
{
    public class SecureLogger<T> : ILogger<T>
    {
        private readonly ILogger<T> logger;
        private readonly IPropertyFilter<JToken> jsonFilter;
        private readonly IPropertyFilter<ObjectToJsonProjection> objectFilter;

        public SecureLogger(ILogger<T> logger, IPropertyFilter<JToken> jsonFilter, IPropertyFilter<ObjectToJsonProjection> objectFilter)
        {
            this.logger = logger;
            this.jsonFilter = jsonFilter;
            this.objectFilter = objectFilter;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this.logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return this.logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.TryParseSecureLoggerState(state, out var secureLoggerState))
            {
                this.logger.Log(logLevel, eventId, state, exception, formatter);
                return;
            }

            try
            {
                this.FilterState<TState>(secureLoggerState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            this.logger.Log(logLevel, eventId, state, exception, formatter);
        }

        private void FilterState<TState>(SecureLoggerState secureLoggerState)
        {
            var dictOfPrimitiveValues = this.FilterPrimitiveValues<TState>(secureLoggerState);

            for (var i = 0; i < secureLoggerState.ValueNames.Count; i++)
            {
                if (secureLoggerState.Values[i] == null)
                {
                    continue;
                }

                var objType = secureLoggerState.Values[i].GetType();
                if ((objType.IsValueType || secureLoggerState.Values[i] is string)
                    && dictOfPrimitiveValues.TryGetValue(secureLoggerState.ValueNames[i], out var value))
                {
                    secureLoggerState.Values[i] = value;
                    continue;
                }

                if (secureLoggerState.Values[i] is IEnumerable)
                {
                    secureLoggerState.Values[i] =
                        this.objectFilter.FilterObject(new { Arr = secureLoggerState.Values[i] })?.Arr!;
                    continue;
                }

                if (!(this.objectFilter.SecureKeyProvider.GetSource<IObjectPropertyFilterSource>() is
                        IObjectPropertyFilterSource source))
                {
                    continue;
                }

                var obj = secureLoggerState.Values[i];
                source.Register(obj.GetType());
                var token = JToken.FromObject(obj);
                var root = new ObjectToJsonProjection(obj, token);
                this.objectFilter.Filter(root);
                secureLoggerState.Values[i] = token.ToObject(obj.GetType());
            }
        }

        private Dictionary<string, object> FilterPrimitiveValues<TState>(SecureLoggerState secureLoggerState)
        {
            var dictOfPrimitiveValues = new Dictionary<string, object>();
            for (var i = 0; i < secureLoggerState.ValueNames.Count; i++)
            {
                if (secureLoggerState.Values[i] == null)
                {
                    continue;
                }

                var objType = secureLoggerState.Values[i].GetType();
                if (objType.IsValueType || secureLoggerState.Values[i] is string)
                {
                    dictOfPrimitiveValues.TryAdd(secureLoggerState.ValueNames[i], secureLoggerState.Values[i]);
                }
            }

            var token = JToken.Parse(JsonConvert.SerializeObject(dictOfPrimitiveValues));
            this.jsonFilter.Filter(token);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(token.ToString());
        }

        private bool TryParseSecureLoggerState<TState>(TState state, out SecureLoggerState secureLoggerState)
        {
            secureLoggerState = default(SecureLoggerState);
            var originalMessageField = state.GetType().GetField("_originalMessage", BindingFlags.Instance | BindingFlags.NonPublic);
            if (originalMessageField == null)
            {
                return false;
            }

            secureLoggerState.OriginalMessage = originalMessageField.GetValue(state) as string;
            if (secureLoggerState.OriginalMessage == null)
            {
                return false;
            }

            if (secureLoggerState.OriginalMessage.IndexOf("{", StringComparison.Ordinal) < 0)
            {
                return false;
            }

            var valuesField = state.GetType().GetField("_values", BindingFlags.Instance | BindingFlags.NonPublic);
            if (valuesField == null)
            {
                return false;
            }

            secureLoggerState.Values = valuesField.GetValue(state) as object[];
            if (secureLoggerState.Values == null || secureLoggerState.Values.Length == 0)
            {
                return false;
            }

            var formatterField = state.GetType().GetField("_formatter", BindingFlags.Instance | BindingFlags.NonPublic);
            if (formatterField == null)
            {
                return false;
            }

            var formatterValue = formatterField.GetValue(state);
            if (formatterValue == null)
            {
                return false;
            }

            var valueNamesField = formatterValue.GetType().GetField("_valueNames", BindingFlags.Instance | BindingFlags.NonPublic);
            if (valueNamesField == null)
            {
                return false;
            }

            secureLoggerState.ValueNames = valueNamesField.GetValue(formatterValue) as List<string>;
            if (secureLoggerState.ValueNames == null || secureLoggerState.ValueNames.Count == 0)
            {
                return false;
            }

            return true;
        }
    }
}
