// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Data;

namespace Gems.Data
{
    /// <summary>
    /// Содержит расширения для <see cref="IDataReader"/>.</summary>
    public static class DbDataReaderExtensions
    {
        /// <summary>
        /// Читает данные из ридера.
        /// </summary>
        /// <typeparam name="T">тип данных.</typeparam>
        /// <param name="reader">текущий ридер.</param>
        /// <param name="ordinal">номер колонки.</param>
        /// <param name="readDataFunc">функция для считывания данных.</param>
        /// <returns>данные из ридера.</returns>
        public static T ReadData<T>(this IDataReader reader, int ordinal, Func<int, T> readDataFunc)
        {
            return reader.IsDBNull(ordinal) ? default : readDataFunc(ordinal);
        }
    }
}
