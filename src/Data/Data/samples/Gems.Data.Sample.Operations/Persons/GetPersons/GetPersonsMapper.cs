// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoMapper;

using Gems.Data.Sample.Operations.Persons.GetPersons.Dto;
using Gems.Data.Sample.Operations.Persons.Shared.Entities;

namespace Gems.Data.Sample.Operations.Persons.GetPersons
{
    public class GetPersonsMapper : Profile
    {
        public GetPersonsMapper()
        {
            this.CreateMap<Person, PersonDto>();
        }
    }
}
