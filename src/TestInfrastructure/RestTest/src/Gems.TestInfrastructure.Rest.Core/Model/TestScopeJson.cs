// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Newtonsoft.Json.Linq;

namespace Gems.TestInfrastructure.Rest.Core.Model
{
    public class TestScopeJson
    {
        public JObject Variables { get; set; }

        public JObject Templates { get; set; }
    }
}
