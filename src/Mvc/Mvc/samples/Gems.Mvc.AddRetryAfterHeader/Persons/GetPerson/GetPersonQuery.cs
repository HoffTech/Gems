// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.AddRetryAfterHeader.Persons.GetPerson.Dto;
using Gems.Mvc.Behaviors;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Mvc.AddRetryAfterHeader.Persons.GetPerson;

public class GetPersonQuery : IRequest<Person>, IRequestAddRetryAfterHeader
{
    [FromRoute]
    public int PersonId { get; set; }
}
