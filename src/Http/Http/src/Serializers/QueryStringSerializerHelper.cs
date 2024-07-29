// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Gems.Text.Json;

using Newtonsoft.Json.Linq;

namespace Gems.Http.Serializers;

public static class QueryStringSerializerHelper
{
    public static async Task<string> SerializeObjectToQueryString(object obj, IList<JsonConverter> serializeAdditionalConverters = null, bool isCamelCase = true)
    {
        if (obj == null)
        {
            return string.Empty;
        }

        if (obj is string queryString)
        {
            return queryString;
        }

        var objAsJson = obj.Serialize(serializeAdditionalConverters, null, isCamelCase);
        return await QueryStringMapper.MapToQueryString(JToken.Parse(objAsJson));
    }
}
