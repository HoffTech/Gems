// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

using Gems.Text.Json.Converters;

namespace Gems.Text.Json
{
    /// <summary>
    /// Расширения для сериализации и десериализации.
    /// </summary>
    public static class SerializeExtensions
    {
        /// <summary>
        /// Выполняет десериализацию JSON строки в указанный объект.
        /// </summary>
        /// <typeparam name="T">тип объекта.</typeparam>
        /// <param name="str">JSON строка.</param>
        /// <param name="additionalConverters">список дополнительных конвертеров.</param>
        /// <returns>T.</returns>
        public static T Deserialize<T>(this string str, IList<JsonConverter> additionalConverters = null)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new DateOnlyJsonConverter());
            if (additionalConverters != null)
            {
                foreach (var converter in additionalConverters)
                {
                    options.Converters.Add(converter);
                }
            }

            return JsonSerializer.Deserialize<T>(str, options);
        }

        /// <summary>
        /// Выполняет сериализацию объекта в JSON строку.
        /// </summary>
        /// <typeparam name="T">тип объекта сериализации.</typeparam>
        /// <param name="type">объект, который нужно сериализовать.</param>
        /// <param name="additionalConverters">список дополнительных конвертеров.</param>
        /// <param name="encoder">объект, который указывает, какие символы кодировщику разрешено не кодировать.</param>
        /// <param name="camelCase">испльзовать CamelCase политику.</param>
        /// <returns>строка содержащая сериализованный объект.</returns>
        public static string Serialize<T>(this T type, IList<JsonConverter> additionalConverters = null, JavaScriptEncoder encoder = null, bool camelCase = false)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = encoder ?? JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            if (camelCase)
            {
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }

            options.Converters.Add(new DateOnlyJsonConverter());
            if (additionalConverters != null)
            {
                foreach (var converter in additionalConverters)
                {
                    options.Converters.Add(converter);
                }
            }

            return JsonSerializer.Serialize(type, options);
        }
    }
}
