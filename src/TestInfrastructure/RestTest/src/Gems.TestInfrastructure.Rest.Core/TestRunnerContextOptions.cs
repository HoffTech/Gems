﻿// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core;

public class TestRunnerContextOptions
{
    public List<string> Namespaces { get; set; }

    public TestRunnerContextFakerOptions Faker { get; set; }
}
