// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Environment
{
    public interface ITestEnvironment : IDisposable, IAsyncDisposable
    {
        object Component(Type type, string name);

        void RegisterComponent(string name, object value, params Type[] types);
    }
}
