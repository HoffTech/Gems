using Gems.Data.Sample.Operations.Persons.CreatePerson.Dto;

using MediatR;

namespace Gems.Data.Sample.Operations.Persons.CreatePerson
{
    public class CreatePersonCommand : IRequest
    {
        public PersonDto Person { get; set; }
    }
}
