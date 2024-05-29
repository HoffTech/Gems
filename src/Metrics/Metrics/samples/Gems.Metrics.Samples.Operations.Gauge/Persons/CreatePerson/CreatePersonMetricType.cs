// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Metrics.Contracts;

namespace Gems.Metrics.Samples.Operations.Gauge.Persons.CreatePerson
{
    public enum CreatePersonMetricType
    {
        [Metric(
            Name = "persons_age",
            Description = "Возраст персоны")]
        PersonAge
    }
}
