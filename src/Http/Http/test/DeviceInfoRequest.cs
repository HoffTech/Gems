// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace Gems.Http.Tests;

public class DeviceInfoRequest
{
    public long Limit { get; set; }

    public long Offset { get; set; }

    [JsonPropertyName("with-phone")]
    public int WithPhone { get; set; }
}
