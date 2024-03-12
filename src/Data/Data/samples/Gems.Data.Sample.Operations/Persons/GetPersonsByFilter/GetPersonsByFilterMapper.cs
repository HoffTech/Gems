using AutoMapper;

using Gems.Data.Sample.Operations.Persons.GetPersonsByFilter.Dto;
using Gems.Data.Sample.Operations.Persons.Shared.Entities;

namespace Gems.Data.Sample.Operations.Persons.GetPersonsByFilter
{
    public class GetPersonsByFilterMapper : Profile
    {
        public GetPersonsByFilterMapper()
        {
            this.CreateMap<Person, PersonDto>();
        }
    }
}
