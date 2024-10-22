// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Hosting;

namespace Gems.TestInfrastructure.Integration
{
    internal class TestApplicationConfiguration
    {
        public List<Action<IWebHostBuilder>> WebHostBuilders { get; } = new List<Action<IWebHostBuilder>>();
    }
}
