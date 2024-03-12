using AutoMapper;

using Gems.Data.Sample.Operations.Persons.CreatePerson.Dto;
using Gems.Data.Sample.Operations.Persons.Shared.Entities;

namespace Gems.Data.Sample.Operations.Persons.CreatePerson
{
    public class CreatePersonMapper : Profile
    {
        public CreatePersonMapper()
        {
            this.CreateMap<PersonDto, Person>();
        }
    }
}
