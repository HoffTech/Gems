// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Sample.Operations.Persons.CreatePerson.Dto;

using MediatR;

namespace Gems.Data.Sample.Operations.Persons.CreatePerson
{
    public class CreatePersonCommand : IRequest
    {
        public PersonDto Person { get; set; }
    }
}
