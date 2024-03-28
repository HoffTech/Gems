// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoMapper;

using Gems.Data.Sample.EFCore.Persons.Entities;
using Gems.Data.Sample.EFCore.Persons.GetPersonById.Dto;

namespace Gems.Data.Sample.EFCore.Persons.GetPersonById
{
    public class GetPersonMapper : Profile
    {
        public GetPersonMapper()
        {
            this.CreateMap<Person, PersonDto>();
        }
    }
}
