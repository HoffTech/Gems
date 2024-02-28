// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.AddRetryAfterHeader.Persons.GetPerson.Dto;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Mvc.AddRetryAfterHeader.Persons.GetPerson;

[Endpoint("api/v1/persons/", "GET", OperationGroup = "Persons")]
public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, Person>
{
    public Task<Person> Handle(GetPersonQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
