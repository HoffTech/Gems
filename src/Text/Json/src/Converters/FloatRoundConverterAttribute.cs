// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace Gems.Text.Json.Converters
{
    public class FloatRoundConverterAttribute : JsonConverterAttribute
    {
        public int DecimalDigitsLength { get; set; }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            if (typeToConvert != typeof(float))
            {
                throw new ArgumentException("float type must be specified.");
            }

            var converter = new FloatRoundConverter
            {
                DecimalDigitsLength = this.DecimalDigitsLength
            };

            return converter;
        }
    }
}
