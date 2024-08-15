// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Authentication.Keycloak.Options;

public class CookieOptions
{
    public string Name { get; set; }

    public int MaxAge { get; set; }

    public double? ExpireTimeSpan { get; set; }
}
