// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;

namespace Gems.Context;

public class ContextAccessor : IContextAccessor
{
    private static readonly AsyncLocal<IContext> CurrentContext = new AsyncLocal<IContext>();

    public IContext Context
    {
        get => CurrentContext.Value;
        set => CurrentContext.Value = value;
    }
}
