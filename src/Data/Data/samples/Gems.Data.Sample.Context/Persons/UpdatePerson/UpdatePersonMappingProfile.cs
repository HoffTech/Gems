using AutoMapper;

using Gems.Data.Sample.Context.Persons.UpdatePerson.Dto;
using Gems.Data.Sample.Context.Persons.UpdatePerson.Entities;

namespace Gems.Data.Sample.Context.Persons.UpdatePerson
{
    public class UpdatePersonMappingProfile : Profile
    {
        public UpdatePersonMappingProfile()
        {
            this.CreateMap<PersonDto, Person>();
        }
    }
}
