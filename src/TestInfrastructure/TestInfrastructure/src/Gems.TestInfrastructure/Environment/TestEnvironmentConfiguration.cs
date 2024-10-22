// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Environment
{
    internal class TestEnvironmentConfiguration
    {
        public List<Func<object>> ComponentBuilders { get; } = new List<Func<object>>();

        public List<Func<ITestEnvironment, CancellationToken, Task>> Bootstrap { get; } = new List<Func<ITestEnvironment, CancellationToken, Task>>();
    }
}
