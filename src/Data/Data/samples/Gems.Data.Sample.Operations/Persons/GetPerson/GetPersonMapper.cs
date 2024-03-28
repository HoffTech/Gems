// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

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
