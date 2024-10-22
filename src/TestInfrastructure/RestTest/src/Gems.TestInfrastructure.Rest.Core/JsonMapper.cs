// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Rest.Core.Model;

using Newtonsoft.Json.Linq;

namespace Gems.TestInfrastructure.Rest.Core
{
    internal static class JsonMapper
    {
        public static Test Map(TestJson test)
        {
            return new Test
            {
                Variables = ConvertToKeyValuePairList(test.Variables),
                Templates = ConvertToKeyValuePairList(test.Templates),
                Output = ConvertToKeyValuePairList(test.Output),
                Author = test.Author,
                Description = test.Description,
                Name = test.Name,
                Asserts = test.Asserts,
                Request = test.Request,
            };
        }

        public static TestCollection Map(TestCollectionJson testCollection)
        {
            return new TestCollection
            {
                Variables = ConvertToKeyValuePairList(testCollection.Variables),
                Templates = ConvertToKeyValuePairList(testCollection.Templates),
                Tests = testCollection.Tests?
                    .Select(x => JsonMapper.Map(x))
                    .ToList(),
            };
        }

        public static TestScope Map(TestScopeJson scope)
        {
            return new TestScope
            {
                Variables = ConvertToKeyValuePairList(scope.Variables),
                Templates = ConvertToKeyValuePairList(scope.Templates),
            };
        }

        public static List<KeyValuePair<string, object>> ConvertToKeyValuePairList(JObject source)
        {
            return source?
                .Properties()
                .Select(x => new KeyValuePair<string, object>(x.Name, x.Value))
                .ToList() ?? new List<KeyValuePair<string, object>>();
        }
    }
}
