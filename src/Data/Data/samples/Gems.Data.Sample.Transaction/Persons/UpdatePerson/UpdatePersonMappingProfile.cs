using AutoMapper;

using Gems.Data.Sample.Transaction.Persons.UpdatePerson.Dto;
using Gems.Data.Sample.Transaction.Persons.UpdatePerson.Entities;

namespace Gems.Data.Sample.Transaction.Persons.UpdatePerson
{
    public class UpdatePersonMappingProfile : Profile
    {
        public UpdatePersonMappingProfile()
        {
            this.CreateMap<PersonDto, Person>();
        }
    }
}
