// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;
using Gems.Mvc.NotFound.Persons.GetPerson.Dto;

using MediatR;

namespace Gems.Mvc.NotFound.Persons.GetPerson;

[Endpoint("api/v1/persons/", "GET", OperationGroup = "Persons")]
public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, Person>
{
    public Task<Person> Handle(GetPersonQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
