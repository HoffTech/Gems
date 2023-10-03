// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Authentication.Keycloak.Options
{
    public class OpenIdConnectOptions
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string MetadataAddress { get; set; }

        public string Authority { get; set; }

        public string SignedOutRedirectUri { get; set; }

        public TokenValidationParameter TokenValidationParameter { get; set; }
    }
}
