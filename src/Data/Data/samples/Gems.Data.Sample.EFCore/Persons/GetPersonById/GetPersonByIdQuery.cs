using System;

using Gems.Data.Sample.EFCore.Persons.GetPersonById.Dto;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.EFCore.Persons.GetPersonById
{
    public class GetPersonByIdQuery : IRequest<PersonDto>
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
    }
}
