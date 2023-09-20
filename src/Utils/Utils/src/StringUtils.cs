// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gems.Utils
{
    /// <summary>
    /// Методы для работы со строками.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Преобразует CamelCase строку в слова.
        /// </summary>
        /// <param name="value">значение в CamelCase регистре.</param>
        /// <returns>Строка со словами.</returns>
        public static string ToFriendlyName(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value.Trim().Length == 0)
            {
                return string.Empty;
            }

            var result = value;

            result = string.Concat(result[..1].ToUpperInvariant(), result.Substring(1, result.Length - 1));

            const string pattern = @"([A-Z]+(?![a-z])|\d+|[A-Z][a-z]+|(?![A-Z])[a-z]+)+";

            var words = new List<string>();
            var match = Regex.Match(result, pattern);
            if (!match.Success)
            {
                return string.Join(" ", words.ToArray());
            }

            var group = match.Groups[1];
            words.AddRange(
                group.Captures.Cast<Capture>()
                    .Select(capture => capture.Value));

            return string.Join(" ", words.ToArray());
        }

        public static string MapUndescoreToSpace(string value)
        {
            return value?.Replace("_", " ");
        }

        public static string MapSpaceToUndescore(string value)
        {
            return value?.Replace(" ", "_");
        }
    }
}
