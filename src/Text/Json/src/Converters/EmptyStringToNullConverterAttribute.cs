// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace Gems.Text.Json.Converters
{
    public class EmptyStringToNullConverterAttribute : JsonConverterAttribute
    {
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            if (typeToConvert != typeof(string))
            {
                throw new ArgumentException("System.String type must be specified.");
            }

            var converter = new EmptyStringToNullConverter();

            return converter;
        }
    }
}
