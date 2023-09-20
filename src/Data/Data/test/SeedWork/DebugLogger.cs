// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

namespace Gems.Data.Tests.SeedWork;

public class DebugLogger : ILogger
{
    private readonly ConcurrentBag<string> logs;

    public DebugLogger(ConcurrentBag<string> logs)
    {
        this.logs = logs;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (formatter != null)
        {
            this.logs.Add(formatter(state, exception));

            Console.WriteLine(formatter(state, exception));
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }
}
