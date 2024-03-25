using System;

namespace Gems.Data.Sample.Metrics.Persons.GetPerson.Dto
{
    public record PersonDto
    {
        public Guid PersonId { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public int Age { get; init; }

        public int Gender { get; init; }
    }
}
