// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gems.Data.MySql
{
    /// <summary>
    /// Extentions for sqls type.
    /// </summary>
    public static class SqlDataTypeExtensions
    {
        /// <summary>
        /// Gets null parameter value.
        /// </summary>
        private const string NullValue = "NULL";

        /// <summary>
        /// Uuid parameter forms.
        /// </summary>
        /// <param name="uid">null or uid.</param>
        /// <returns>uuid parameter.</returns>
        public static string GetSqlParameters(this Guid? uid)
        {
            var uuid = uid == null || uid == Guid.Empty ? NullValue : $"'{uid}'";

            return uuid;
        }

        /// <summary>
        /// Character parameter forms.
        /// </summary>
        /// <param name="s">string parameter.</param>
        /// <returns>nullable character varying parameter.</returns>
        public static string GetSqlParameters(this string s)
        {
            var characterParameter = string.IsNullOrEmpty(s) ? NullValue : $"'{s}'";

            return characterParameter;
        }

        /// <summary>
        /// Form text array parameter from List<typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">generic type.</typeparam>
        /// <param name="list">list of T-elements.</param>
        /// <returns>text array parameter.</returns>
        public static string GetSqlParameters<T>(this List<T> list)
        {
            string textArray;

            if (list == null)
            {
                textArray = NullValue;
            }
            else
            {
                textArray = list.Aggregate("'{", (current, value) => current + $"\"{value}\",");

                textArray = textArray.Substring(0, textArray.Length - 1) + "}'";
            }

            return textArray;
        }
    }
}
