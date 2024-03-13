using System;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetPersonAge
{
    public class GetPersonAgeQuery : IRequest<int>
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
    }
}
