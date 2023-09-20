// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Utils.Attributes
{
    /// <summary>
    /// Аттрибут для назначения меток перечислениям.
    /// </summary>
    [Obsolete("Use MetricAttribute")]
    [AttributeUsage(AttributeTargets.All)]
    public class LabelValuesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LabelValuesAttribute"/> class.
        /// </summary>
        /// <param name="labelValues">Метки перечисления.</param>
        public LabelValuesAttribute(params string[] labelValues)
        {
            this.LabelValues = labelValues;
        }

        public string[] LabelValues { get; }
    }
}
