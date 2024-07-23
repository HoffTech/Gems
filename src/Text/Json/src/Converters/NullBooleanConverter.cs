// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gems.Text.Json.Converters;

public class NullBooleanConverter : JsonConverter<bool>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(bool) || typeToConvert == typeof(bool?);
    }

    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return reader.GetBoolean();
        }
        catch
        {
            return default;
        }
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}
