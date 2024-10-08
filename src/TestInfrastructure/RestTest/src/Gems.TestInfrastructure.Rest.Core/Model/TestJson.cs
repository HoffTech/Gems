﻿using Newtonsoft.Json.Linq;

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
