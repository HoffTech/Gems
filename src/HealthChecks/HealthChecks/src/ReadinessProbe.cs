// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.HealthChecks
{
    internal class ReadinessProbe : IReadinessProbe
    {
        public ReadinessProbe()
        {
            this.ServiceIsReady = true;
        }

        public ReadinessProbe(bool serviceIsReady)
        {
            this.ServiceIsReady = serviceIsReady;
        }

        public bool ServiceIsReady { get; set; }
    }
}
