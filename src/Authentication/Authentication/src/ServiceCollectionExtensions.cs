// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Gems.Authentication.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Gems.Authentication
{
    /// <summary>
    /// Class with middleware extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Class with middleware extensions.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        /// <param name="configuration">IConfiguration.</param>
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ADOptions>(configuration.GetSection(ADOptions.AD));
            var adOptions = configuration.GetSection(ADOptions.AD).Get<ADOptions>();

            var certificate = new X509Certificate2(adOptions.CertFileName);
            var key = new X509SecurityKey(certificate);

            var tokenValidationParameters = new TokenValidationParameters
            {
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = false,
                ValidIssuer = adOptions.ValidIssuer,
                ValidIssuers = new List<string>() { adOptions.Authority },

                // Validate the JWT Audience (aud) claim
                ValidateAudience = false,
                ValidAudience = adOptions.ClientId,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
            };

            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.Authority = adOptions.Authority;
                    options.Audience = adOptions.ClientId;
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = tokenValidationParameters;

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = ctx =>
                        {
                            ctx.Principal?.AddIdentity(new ClaimsIdentity(new List<Claim>()));

                            return Task.CompletedTask;
                        },
                    };
                });
        }
    }
}
