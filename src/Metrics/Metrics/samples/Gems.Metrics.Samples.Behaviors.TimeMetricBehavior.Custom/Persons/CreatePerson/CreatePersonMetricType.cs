// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Metrics.Contracts;

namespace Gems.Metrics.Samples.Behaviors.TimeMetricBehavior.Custom.Persons.CreatePerson
{
    public enum CreatePersonMetricType
    {
        [Metric(
            Name = "create_person_custom_time_metric",
            Description = "Создание персоны уникальная временная метрика")]
        CreatePersonTime
    }
}
