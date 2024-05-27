// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

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
