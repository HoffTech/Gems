// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Data.Sample.Metrics.Persons.GetPerson.Dto;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Metrics.Persons.GetPerson
{
    public class GetPersonQuery : IRequest<PersonDto>
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
    }
}
