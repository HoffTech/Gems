// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Metrics.Contracts;

namespace Gems.Metrics.Samples.Labels.Persons.ImportPersons
{
    public enum ImportPersonsMetricType
    {
        [Metric(
            Name = "import_persons",
            Description = "Импорт персон",
            LabelNames = new[] { "operation_type" })]
        ImportPersonCounters
    }
}
