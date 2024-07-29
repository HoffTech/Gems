// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Gems.Text.Json;

namespace Gems.Http.Serializers;

public static class JsonSerializerHelper
{
    private static Type[] ScalarStructureTypes => new Type[] { typeof(DateTime), typeof(DateOnly), typeof(Guid), typeof(DateTimeOffset) };

    public static string SerializeObjectToJson(object obj, IList<JsonConverter> serializeAdditionalConverters = null, bool isCamelCase = true)
    {
        if (obj is null)
        {
            return null;
        }

        if (obj is string json)
        {
            return json;
        }

        return obj.Serialize(serializeAdditionalConverters, null, isCamelCase);
    }

    public static TResponse DeserializeObjectFromJson<TResponse>(string json, IList<JsonConverter> deserializeAdditionalConverters = null, Type[] additionalScalarStructureTypes = null)
    {
        if (typeof(TResponse) == typeof(string) && !json.StartsWith('"') && !json.EndsWith('"'))
        {
            return (TResponse)((object)json);
        }

        if (!IsNumericOrDecimal(typeof(TResponse))
            && IsScalarStructure(typeof(TResponse), additionalScalarStructureTypes)
            && !json.StartsWith('"'))
        {
            json = '"' + json + '"';
        }

        return json.Deserialize<TResponse>(deserializeAdditionalConverters);
    }

    private static bool IsNumericOrDecimal(Type type)
    {
        return type.IsPrimitive || type == typeof(decimal);
    }

    private static bool IsScalarStructure(Type type, Type[] additionalScalarStructureTypes)
    {
        return ScalarStructureTypes.Contains(type)
               || (additionalScalarStructureTypes ?? Array.Empty<Type>()).Contains(type);
    }
}
