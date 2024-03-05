// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Behaviors;
using Gems.Mvc.Sample.HandlersUsing.Persons.CreatePerson.Entities;

using MediatR;

namespace Gems.Mvc.Sample.HandlersUsing.Persons.CreatePerson;

public class CreatePersonCommand : IRequest<Guid>, IRequestUnitOfWork
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int Age { get; set; }

    public Gender Gender { get; set; }
}
