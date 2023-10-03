// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Authentication.Keycloak.Options;

public class KeycloakAuthOptions
{
    public const string SectionName = "KeycloakAuthOptions";

    public CookieOptions CookieOptions { get; set; }

    public OpenIdConnectOptions OpenIdConnectOptions { get; set; }
}
