// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Newtonsoft.Json;

namespace Gems.Logging.Mvc.LogsCollector;

public class EnumConverter : JsonConverter<Enum>
{
    public override void WriteJson(JsonWriter writer, Enum value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override Enum ReadJson(JsonReader reader, Type objectType, Enum existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return existingValue ?? default;
    }
}
