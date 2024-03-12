using AutoMapper;

using Gems.Data.Sample.Operations.Persons.GetPerson.Dto;
using Gems.Data.Sample.Operations.Persons.Shared.Entities;

namespace Gems.Data.Sample.Operations.Persons.GetPerson
{
    public class GetPersonMapper : Profile
    {
        public GetPersonMapper()
        {
            this.CreateMap<Person, PersonDto>();
        }
    }
}
