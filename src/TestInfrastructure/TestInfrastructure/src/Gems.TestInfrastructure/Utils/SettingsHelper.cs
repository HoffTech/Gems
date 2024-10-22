// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gems.TestInfrastructure.Utils
{
    public static class SettingsHelper
    {
        public static List<KeyValuePair<string, string>> FlattenObject<T>(T value, string name)
        {
            var result = new List<KeyValuePair<string, string>>();
            var jObject = JToken.FromObject(value);
            var names = new Stack<string>();
            if (!string.IsNullOrEmpty(name))
            {
                names.Push(name);
            }

            VisitJToken(jObject, names, result);
            return result;
        }

        private static void VisitJToken(
            JToken root,
            Stack<string> names,
            List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (root == null)
            {
                return;
            }
            else if (root is JObject jObject)
            {
                foreach (var prop in jObject.Properties())
                {
                    VisitJToken(prop, names, keyValuePairs);
                }
            }
            else if (root is JArray jArray)
            {
                if (jArray.HasValues)
                {
                    var index = 0;
                    foreach (var jArrayValue in jArray.Values())
                    {
                        names.Push(index.ToString());
                        VisitJToken(jArrayValue, names, keyValuePairs);
                        names.Pop();
                        index++;
                    }
                }
            }
            else if (root is JValue jValue)
            {
                if (jValue.Value != null)
                {
                    var kv = new KeyValuePair<string, string>(
                        string.Join(":", names.Reverse()),
                        ConvertToString(jValue));
                    keyValuePairs.Add(kv);
                }
            }
            else if (root is JProperty jProperty)
            {
                var value = jProperty.Value;
                if (value != null)
                {
                    names.Push(jProperty.Name);
                    VisitJToken(value, names, keyValuePairs);
                    names.Pop();
                }
            }
        }

        private static string ConvertToString(JValue jValue)
        {
            if (jValue.Type == JTokenType.String)
            {
                return (string)jValue.Value;
            }
            else
            {
                return JsonConvert.SerializeObject(jValue.Value);
            }
        }
    }
}
