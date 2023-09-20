// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Gems.Http;

public static class QueryStringMapper
{
    public static async Task<string> MapToQueryString(JToken data)
    {
        var keyValueContent = data.ToKeyValue();
        var formUrlEncodedContent = new FormUrlEncodedContent(keyValueContent);
        var urlEncodedString = await formUrlEncodedContent.ReadAsStringAsync();
        return urlEncodedString;
    }

    private static IDictionary<string, string> ToKeyValue(this JToken token)
    {
        if (token == null)
        {
            return null;
        }

        if (token.HasValues)
        {
            var contentData = new Dictionary<string, string>();
            foreach (var child in token.Children().ToList())
            {
                var childContent = child.ToKeyValue();
                if (childContent != null)
                {
                    contentData = contentData.Concat(childContent)
                        .ToDictionary(k => k.Key, v => v.Value);
                }
            }

            return contentData;
        }

        var jValue = token as JValue;
        if (jValue?.Value == null)
        {
            return null;
        }

        var value = jValue?.Type == JTokenType.Date ?
            jValue?.ToString("o", CultureInfo.InvariantCulture) :
            jValue?.ToString(CultureInfo.InvariantCulture);

        return new Dictionary<string, string> { { token.Path, value } };
    }
}
