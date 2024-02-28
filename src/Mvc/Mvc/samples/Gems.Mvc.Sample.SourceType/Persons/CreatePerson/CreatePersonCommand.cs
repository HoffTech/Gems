// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Behaviors;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Mvc.Sample.SourceType.Persons.CreatePerson;

public class CreatePersonCommand : IRequest<Guid>, IRequestUnitOfWork
{
    [FromQuery(Name = "firstName")]
    public string FirstName { get; set; }

    [FromQuery(Name = "lastName")]
    public string LastName { get; set; }

    [FromQuery(Name = "age")]
    public int Age { get; set; }
}
