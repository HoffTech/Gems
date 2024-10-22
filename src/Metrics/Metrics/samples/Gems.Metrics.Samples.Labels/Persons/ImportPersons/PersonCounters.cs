// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Metrics.Samples.Labels.Persons.ImportPersons
{
    public record PersonCounters
    {
        public int Added { get; init; }

        public int Updated { get; init; }

        public int Deleted { get; init; }
    }
}
