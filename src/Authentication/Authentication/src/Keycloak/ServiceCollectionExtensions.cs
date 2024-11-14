// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

using Gems.Authentication.Keycloak.Options;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Gems.Authentication.Keycloak;

/// <summary>
/// Class with middleware extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add keycloak code flow authentication.
    /// </summary>
    /// <param name="services">IServiceCollection.</param>
    /// <param name="configuration">IConfiguration.</param>
    public static void AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var keycloakAuthOptions = configuration.GetSection(KeycloakAuthOptions.SectionName).Get<KeycloakAuthOptions>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(cookie =>
            {
                cookie.Cookie.Name = keycloakAuthOptions.CookieOptions.Name;
                cookie.Cookie.MaxAge = TimeSpan.FromMinutes(keycloakAuthOptions.CookieOptions.MaxAge);
                cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                cookie.SlidingExpiration = true;
                if (keycloakAuthOptions.CookieOptions?.ExpireTimeSpan != null)
                {
                    cookie.ExpireTimeSpan = TimeSpan.FromMinutes((double)keycloakAuthOptions.CookieOptions.ExpireTimeSpan);
                }
            })
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = keycloakAuthOptions.OpenIdConnectOptions.Authority;

                options.SignedOutRedirectUri = keycloakAuthOptions.OpenIdConnectOptions.SignedOutRedirectUri;
                options.ClientId = keycloakAuthOptions.OpenIdConnectOptions.ClientId;
                options.ClientSecret = keycloakAuthOptions.OpenIdConnectOptions.ClientSecret;
                options.MetadataAddress = keycloakAuthOptions.OpenIdConnectOptions.MetadataAddress;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = keycloakAuthOptions.OpenIdConnectOptions.RequireHttpsMetadata;
                options.NonceCookie.SameSite = SameSiteMode.Unspecified;
                options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = keycloakAuthOptions.OpenIdConnectOptions.TokenValidationParameter.NameClaimType
                };
                options.Events.OnRedirectToIdentityProvider = context =>
                {
                    context.Request.Scheme = "https";
                    return Task.CompletedTask;
                };
            });
    }
}
