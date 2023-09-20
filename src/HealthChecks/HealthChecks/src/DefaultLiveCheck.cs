// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Gems.HealthChecks
{
    internal class DefaultLiveCheck : IHealthCheck
    {
        private readonly ILivenessProbe probe;

        public DefaultLiveCheck(ILivenessProbe probe)
        {
            this.probe = probe;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.probe.ServiceIsAlive ? HealthCheckResult.Healthy("OK") : HealthCheckResult.Unhealthy());
        }
    }
}
