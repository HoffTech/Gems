// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gems.Text.Json.Converters;

public class EmptyStringToNullConverter : JsonConverter<string>
{
    public override bool HandleNull => true;

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(string);
    }

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value == string.Empty ? null : value;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (value == string.Empty)
        {
            value = null;
        }

        writer.WriteStringValue(value);
    }
}
