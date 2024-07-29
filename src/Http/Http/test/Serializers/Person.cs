// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Http.Tests.Serializers;

public record Person
{
    public string Name { get; init; }

    public int Age { get; init; }

    public DateTime Birthday { get; init; }

    public bool IsMale { get; init; }
}

public struct PersonStruct
{
    public string Name { get; init; }

    public int Age { get; init; }

    public DateTime Birthday { get; init; }

    public bool IsMale { get; init; }
}
