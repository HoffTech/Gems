// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Metrics.Http
{
    public interface IStatusCodeMetricAvailable
    {
        public Enum StatusCodeMetricType { get; }
    }
}
