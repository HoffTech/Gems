// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace Gems.Text.Json.Converters
{
    public class NullBooleanConverterAttribute : JsonConverterAttribute
    {
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            if (typeToConvert != typeof(bool) && typeToConvert != typeof(bool?))
            {
                throw new ArgumentException("bool type must be specified.");
            }

            var converter = new NullBooleanConverter();

            return converter;
        }
    }
}
