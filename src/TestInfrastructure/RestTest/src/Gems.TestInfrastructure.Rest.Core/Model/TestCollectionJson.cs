// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core.Model;

public class TestCollectionJson : TestScopeJson
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Author { get; set; }

    public List<TestJson> Tests { get; set; }
}
