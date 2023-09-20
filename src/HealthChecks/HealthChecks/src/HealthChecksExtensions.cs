// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.HealthChecks
{
    public static class HealthChecksExtensions
    {
        public static IHealthChecksBuilder AddDefaultHealthChecks(this IServiceCollection services, Action<ProbeOptions> defaults = null)
        {
            var op = new ProbeOptions();
            defaults?.Invoke(op);
            services.AddSingleton<ILivenessProbe, LivenessProbe>(_ => new LivenessProbe(op.DefaultServiceIsAlive));
            services.AddSingleton<IReadinessProbe, ReadinessProbe>(_ => new ReadinessProbe(op.DefaultServiceIsReady));
            return services
                .AddHealthChecks()
                .AddCheck<DefaultLiveCheck>("DefaultLive", tags: new[] { "live" })
                .AddCheck<DefaultReadyCheck>("DefaultReady", tags: new[] { "ready" });
        }

        public static IEndpointRouteBuilder MapDefaultHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks(
                "/liveness",
                new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                {
                    Predicate = h => h.Tags.Contains("live"),
                });
            endpoints.MapHealthChecks(
                "/readiness",
                new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                {
                    Predicate = h => h.Tags.Contains("ready"),
                });
            return endpoints;
        }
    }
}
