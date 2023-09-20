// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.ComponentModel;

using Gems.Utils.Attributes;

namespace Gems.Utils
{
    /// <summary>
    /// Методы для работы с перечислениями.
    /// </summary>
    public static class EnumUtils
    {
        private static readonly ConcurrentDictionary<Enum, string> Descriptions = new ConcurrentDictionary<Enum, string>();
        private static readonly ConcurrentDictionary<Enum, string> FriendlyNames = new ConcurrentDictionary<Enum, string>();
        private static readonly ConcurrentDictionary<Enum, string[]> LabelValues = new ConcurrentDictionary<Enum, string[]>();

        /// <summary>
        /// Получает описание из аттрибута Description.
        /// </summary>
        /// <param name="enum">значение перечисления.</param>
        /// <returns>Описание перечисления.</returns>
        [Obsolete("Use MetricAttribute")]
        public static string ToDescription(this Enum @enum)
        {
            if (@enum == null)
            {
                return string.Empty;
            }

            if (!Enum.IsDefined(@enum.GetType(), @enum))
            {
                return string.Empty;
            }

            if (Descriptions.TryGetValue(@enum, out var description))
            {
                return description;
            }

            var fieldInfo = @enum.GetType().GetField(@enum.ToString());
            if (fieldInfo == null)
            {
                return StringUtils.ToFriendlyName(@enum.ToString());
            }

            if (!(fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[]
                    attributes) || attributes.Length <= 0)
            {
                return StringUtils.ToFriendlyName(@enum.ToString());
            }

            description = attributes[0].Description;
            Descriptions.TryAdd(@enum, description);
            return description;
        }

        /// <summary>
        /// Получает описание из аттрибута FriendlyName.
        /// </summary>
        /// <param name="enum">значение перечисления.</param>
        /// <returns>Описание перечисления.</returns>
        [Obsolete("Use MetricAttribute")]
        public static string ToFriendlyName(this Enum @enum)
        {
            if (@enum == null)
            {
                return string.Empty;
            }

            if (!Enum.IsDefined(@enum.GetType(), @enum))
            {
                return string.Empty;
            }

            if (FriendlyNames.TryGetValue(@enum, out var friendlyName))
            {
                return friendlyName;
            }

            var fieldInfo = @enum.GetType().GetField(@enum.ToString());
            if (fieldInfo == null)
            {
                return StringUtils.ToFriendlyName(@enum.ToString());
            }

            if (!(fieldInfo.GetCustomAttributes(typeof(FriendlyNameAttribute), false) is FriendlyNameAttribute[]
                    attributes) || attributes.Length <= 0)
            {
                return StringUtils.ToFriendlyName(@enum.ToString());
            }

            friendlyName = attributes[0].Name;
            FriendlyNames.TryAdd(@enum, friendlyName);
            return friendlyName;
        }

        /// <summary>
        /// Получает метки из аттрибута LabelValuesAttribute.
        /// </summary>
        /// <param name="enum">значение перечисления.</param>
        /// <returns>Описание перечисления.</returns>
        [Obsolete("Use MetricAttribute")]
        public static string[] GetLabelValues(Enum @enum)
        {
            if (@enum == null)
            {
                return Array.Empty<string>();
            }

            if (!Enum.IsDefined(@enum.GetType(), @enum))
            {
                return Array.Empty<string>();
            }

            if (LabelValues.TryGetValue(@enum, out var labelValues))
            {
                return labelValues;
            }

            var fieldInfo = @enum.GetType().GetField(@enum.ToString());
            if (fieldInfo == null)
            {
                return Array.Empty<string>();
            }

            if (!(fieldInfo.GetCustomAttributes(typeof(LabelValuesAttribute), false) is LabelValuesAttribute[]
                    attributes) || attributes.Length <= 0)
            {
                return Array.Empty<string>();
            }

            labelValues = attributes[0].LabelValues;
            LabelValues.TryAdd(@enum, labelValues);
            return labelValues;
        }
    }
}
