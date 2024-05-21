// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gems.Text.Json.Converters;

public class DoubleRoundConverter : JsonConverter<double>
{
    public int DecimalDigitsLength { get; init; }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(double);
    }

    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDouble();
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Math.Round(value, this.DecimalDigitsLength));
    }
}
