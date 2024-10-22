// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core.Model;

public class HttpRequestDefinition
{
    public HttpMethod Method { get; set; }

    public Uri Address { get; set; }

    public object Body { get; set; }

    public Dictionary<string, string> Headers { get; set; }

    public TimeSpan? Timeout { get; set; }
}
