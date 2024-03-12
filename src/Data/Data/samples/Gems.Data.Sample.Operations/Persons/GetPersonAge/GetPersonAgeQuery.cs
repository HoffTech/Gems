using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetPersonAge
{
    public class GetPersonAgeQuery : IRequest<int>
    {
        [FromRoute(Name = "id")]
        public string Id { get; set; }
    }
}
