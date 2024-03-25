using AutoMapper;

using Gems.Data.Sample.Metrics.Persons.GetPerson.Dto;
using Gems.Data.Sample.Metrics.Persons.GetPerson.Entities;

namespace Gems.Data.Sample.Metrics.Persons.GetPerson
{
    public class GetPersonMapper : Profile
    {
        public GetPersonMapper()
        {
            this.CreateMap<Person, PersonDto>();
        }
    }
}
