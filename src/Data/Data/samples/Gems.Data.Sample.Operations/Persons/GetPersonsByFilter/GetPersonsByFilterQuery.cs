using System.Collections.Generic;

using Gems.Data.Sample.Operations.Persons.GetPersonsByFilter.Dto;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetPersonsByFilter
{
    public class GetPersonsByFilterQuery : IRequest<List<PersonDto>>
    {
        [FromQuery]
        public string FirstName { get; set; }

        [FromQuery]
        public string LastName { get; set; }

        [FromQuery]
        public int? Age { get; set; }

        [FromQuery]
        public int? Skip { get; set; }

        [FromQuery]
        public int? Take { get; set; }
    }
}
