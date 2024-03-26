// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Data.Sample.Operations.Persons.GetPerson.Dto;
using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetPerson
{
    [Endpoint("api/v1/persons/{id}", "GET", OperationGroup = "Persons")]
    public class GetPersonQuery : IRequest<PersonDto>
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
    }
}
