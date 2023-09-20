// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.IO.Attributes
{
    /// <summary>
    /// Класс-атрибут для обозначения заголовков полей коллекции.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnHeaderNameAttribute : Attribute
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ColumnHeaderNameAttribute"/>.
        /// </summary>
        public ColumnHeaderNameAttribute(string headerName)
        {
            this.HeaderName = headerName;
        }

        /// <summary>
        /// Наименование заголовка.
        /// </summary>
        public virtual string HeaderName { get; }
    }
}
