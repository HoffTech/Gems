// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.HealthChecks
{
    internal class LivenessProbe : ILivenessProbe
    {
        public LivenessProbe()
        {
            this.ServiceIsAlive = true;
        }

        public LivenessProbe(bool serviceIsAlive)
        {
            this.ServiceIsAlive = serviceIsAlive;
        }

        public bool ServiceIsAlive { get; set; }
    }
}
