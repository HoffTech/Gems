// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Metrics.Samples.Labels.Persons.ImportPersons;

namespace Gems.Metrics.Samples.Labels.Persons.CreatePerson.Dto
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
