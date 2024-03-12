using System;

using Gems.Data.Sample.Operations.Persons.GetPerson.Dto;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetPerson
{
    public class GetPersonQuery : IRequest<PersonDto>
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
    }
}
