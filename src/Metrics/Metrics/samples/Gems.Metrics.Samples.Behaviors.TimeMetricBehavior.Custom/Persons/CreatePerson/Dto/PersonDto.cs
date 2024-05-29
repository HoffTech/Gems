// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Metrics.Samples.Behaviors.TimeMetricBehavior.Custom.Persons.CreatePerson.Dto
{
    public record PersonDto
    {
        public Guid PersonId { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public int Age { get; init; }

        public Gender Gender { get; init; }
    }
}
