// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core;

public class TestRunnerOptions
{
    public TestRunnerContextOptions Context { get; set; }

    public HttpClient HttpClient { get; set; }

    public static TestRunnerOptions CreateDefault()
    {
        return new TestRunnerOptions
        {
            Context = new TestRunnerContextOptions
            {
                Namespaces = new List<string> { "System", },
                Faker = new TestRunnerContextFakerOptions() { Locale = "ru", },
            }
        };
    }
}
