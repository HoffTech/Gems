// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core.Model
{
    public class TestScope
    {
        public List<KeyValuePair<string, object>> Variables { get; set; }

        public List<KeyValuePair<string, object>> Templates { get; set; }
    }
}
