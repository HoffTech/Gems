// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core;

public class TestRunnerContextImportOptions
{
    public TestRunnerContextImportOptions(Type type, string name = default)
    {
        this.Type = type;
        this.Name = name;
    }

    public Type Type { get; set; }

    public string Name { get; set; }
}
