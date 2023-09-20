// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Concurrent;

namespace Gems.Context;

public class DefaultContext : IContext
{
    public ConcurrentDictionary<object, object> Items { get; set; } = new ConcurrentDictionary<object, object>();
}
