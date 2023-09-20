// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Jobs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JobHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobHandlerAttribute"/> class.
        /// </summary>
        /// <param name="name">Наименование задания, которое будет выступать в качестве идентификатора.</param>
        /// <param name="isConcurrent">Признак разрешения запуска задачи в нескольких потоках.</param>
        public JobHandlerAttribute(string name, bool isConcurrent = false)
        {
            this.Name = name;
            this.IsConcurrent = isConcurrent;
        }

        /// <summary>
        /// Gets наименование задания.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether gets: Признак разрешения запуска задачи в нескольких потоках.
        /// </summary>
        public bool IsConcurrent { get; }
    }
}
