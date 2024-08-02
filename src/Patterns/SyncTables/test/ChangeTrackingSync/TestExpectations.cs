// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Patterns.SyncTables.Tests.ChangeTrackingSync;

public class TestExpectations
{
    public long? NewVersion { get; set; }

    public bool IsFullLoad { get; set; }

    public Type ExceptionType { get; set; }

    public int? BatchSize { get; set; }

    public int? LoadIterations { get; set; }

    public int? SaveIterations { get; set; }
}
