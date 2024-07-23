// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace Gems.Text.Json.Converters
{
    public class NullDoubleConverterAttribute : JsonConverterAttribute
    {
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            if (typeToConvert != typeof(double) && typeToConvert != typeof(double?))
            {
                throw new ArgumentException("double type must be specified.");
            }

            var converter = new NullDoubleConverter();

            return converter;
        }
    }
}
