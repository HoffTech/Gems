using System;

namespace Gems.Data.Sample.EFCore.Persons.GetPersonById.Dto
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
