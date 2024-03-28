// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Gems.Data.Sample.Operations.Persons.GetPersons.Dto;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetPersons
{
    public class GetPersonsQuery : IRequest<List<PersonDto>>
    {
        [FromQuery]
        public int? Skip { get; set; }

        [FromQuery]
        public int? Take { get; set; }
    }
}
