// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;

namespace Gems.Logging.Serilog
{
    /// <summary>
    /// An <see cref="ITextFormatter"/> that writes events in a compact JSON format.
    /// </summary>
    public class CompactJsonFormatter : ITextFormatter
    {
        private readonly string[] pathValueNames = new string[]
        {
            "RequestPath",
            "requestPath"
        };

        private readonly string[] sysPaths = new string[]
        {
            "/metrics",
            "/health",
            "/liveness",
            "/readiness",
            "/dashboard",
            "/quartz"
        };

        private readonly JsonValueFormatter valueFormatter;
        private readonly JsonSerializer jsonSerializer;

        /// <summary>
        /// Construct a <see cref="CompactJsonFormatter"/>, optionally supplying a formatter for
        /// <see cref="LogEventPropertyValue"/>s on the event.
        /// </summary>
        /// <param name="valueFormatter">A value formatter, or null.</param>
        public CompactJsonFormatter(JsonValueFormatter valueFormatter = null)
        {
            this.valueFormatter = valueFormatter ?? new JsonValueFormatter(typeTagName: "Type");
            this.jsonSerializer = new JsonSerializer();
            this.jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            this.jsonSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        /// <summary>
        /// Format the log event into the output. Subsequent events will be newline-delimited.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (this.Filter(logEvent))
            {
                this.FormatEvent(logEvent, output, this.valueFormatter);
                output.WriteLine();
            }
        }

        private static string GetLogEventLevel(LogEventLevel level)
        {
            return level switch
            {
                LogEventLevel.Verbose => "Verbose",
                LogEventLevel.Debug => "Debug",
                LogEventLevel.Information => "Info",
                LogEventLevel.Warning => "Warn",
                LogEventLevel.Error => "Error",
                LogEventLevel.Fatal => "Fatal",
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
        }

        private static void Normalize(JToken token)
        {
            foreach (var c in token.Children().ToList())
            {
                if (c is JProperty prop)
                {
                    if (IsNullOrEmpty(prop.Value))
                    {
                        prop.Remove();
                        continue;
                    }

                    if (prop.Name.StartsWith("RemoteStack"))
                    {
                        prop.Remove();
                        continue;
                    }

                    if (prop.Name == "ClassName")
                    {
                        var propValue = prop.Value.ToString();
                        var newProp = new JProperty("Type", propValue);
                        prop.Replace(newProp);
                    }

                    if (prop.Value is JObject)
                    {
                        Normalize(prop.Value);
                    }
                }
            }
        }

        private static bool IsNullOrEmpty(JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == string.Empty) ||
                   (token.Type == JTokenType.Null);
        }

        private static bool PathStartsWithSegments(string path, string sergments)
        {
            return
                path.Equals(sergments, StringComparison.InvariantCultureIgnoreCase) ||
                path.StartsWith(sergments + "/", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Format the log event into the output.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        /// <param name="valueFormatter">A value formatter for <see cref="LogEventPropertyValue"/>s on the event.</param>
        private void FormatEvent(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (valueFormatter == null)
            {
                throw new ArgumentNullException(nameof(valueFormatter));
            }

            output.Write("{ \"@Timestamp\": \"");
            output.Write(logEvent.Timestamp.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss.ffff"));

            // Level
            output.Write("\", \"@level\": \"");
            output.Write(GetLogEventLevel(logEvent.Level));

            // Host
            output.Write("\", \"@host\": \"");
            output.Write(Environment.MachineName);

            // MessageTemplate
            output.Write("\", \"MessageTemplate\": ");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);

            var tokensWithFormat = logEvent.MessageTemplate.Tokens
                .OfType<PropertyToken>()
                .Where(pt => pt.Format != null);

            // Better not to allocate an array in the 99.9% of cases where this is false
            // ReSharper disable once PossibleMultipleEnumeration
            if (tokensWithFormat.Any())
            {
                output.Write(", \"@r\":[");
                var delim = string.Empty;
                foreach (var r in tokensWithFormat)
                {
                    output.Write(delim);
                    delim = ",";
                    var space = new StringWriter();
                    r.Render(logEvent.Properties, space);
                    JsonValueFormatter.WriteQuotedJsonString(space.ToString(), output);
                }

                output.Write(']');
            }

            if (logEvent.Exception != null)
            {
                output.Write(", \"@exception\": ");
                var value = this.FormatException(logEvent.Exception);
                output.Write(value);
            }

            foreach (var property in logEvent.Properties)
            {
                var name = property.Key;
                if (name.Length > 0 && name[0] == '@')
                {
                    // Escape first '@' by doubling
                    name = '@' + name;
                }

                output.Write(", ");
                JsonValueFormatter.WriteQuotedJsonString(name, output);
                output.Write(": ");
                valueFormatter.Format(property.Value, output);
            }

            output.Write(" }");
        }

        private string FormatException(Exception ex)
        {
            var token = JToken.FromObject(ex, this.jsonSerializer);
            Normalize(token);
            return token.ToString(Formatting.None);
        }

        private bool Filter(LogEvent logEvent)
        {
            var path = string.Empty;
            LogEventPropertyValue pathValue = null;

            foreach (var pathValueName in this.pathValueNames)
            {
                if (logEvent.Properties.TryGetValue(pathValueName, out pathValue))
                {
                    break;
                }
            }

            if (pathValue is ScalarValue scalarPathValue)
            {
                path = scalarPathValue.Value?.ToString();
            }

            if (string.IsNullOrEmpty(path))
            {
                return true;
            }

            return !this.sysPaths.Any(x => PathStartsWithSegments(path, x));
        }
    }
}
