// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Newtonsoft.Json.Linq;

namespace Gems.TestInfrastructure.Rest.Core.Model;

public class TestJson : TestScopeJson
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Author { get; set; }

    public TestRequest Request { get; set; }

    public List<object> Asserts { get; set; }

    public JObject Output { get; set; }
}
