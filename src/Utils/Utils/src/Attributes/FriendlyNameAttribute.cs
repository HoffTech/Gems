// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Utils.Attributes
{
    /// <summary>
    /// Аттрибут для назначения имени перечислениям.
    /// </summary>
    [Obsolete("Use MetricAttribute")]
    [AttributeUsage(AttributeTargets.All)]
    public class FriendlyNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendlyNameAttribute"/> class.
        /// </summary>
        /// <param name="name">Имя перечисления.</param>
        public FriendlyNameAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets Имя.
        /// </summary>
        public string Name { get; }
    }
}
