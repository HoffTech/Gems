// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

namespace Gems.Data.Tests.SeedWork;

public class DebugLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentBag<string> logs;
    private readonly ConcurrentDictionary<string, ILogger> loggers;

    public DebugLoggerProvider(ConcurrentBag<string> logs)
    {
        this.logs = logs;
        this.loggers = new ConcurrentDictionary<string, ILogger>();
    }

    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return this.loggers.GetOrAdd(categoryName, new DebugLogger(this.logs));
    }
}
