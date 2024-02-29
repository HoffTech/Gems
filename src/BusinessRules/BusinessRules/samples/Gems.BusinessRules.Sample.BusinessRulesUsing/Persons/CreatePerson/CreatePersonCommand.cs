// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.BusinessRules.Sample.BusinessRulesUsing.Persons.Shared.Entities;
using Gems.Data.Behaviors;

using MediatR;

namespace Gems.BusinessRules.Sample.BusinessRulesUsing.Persons.CreatePerson;

public class CreatePersonCommand : IRequest<Guid>, IRequestUnitOfWork
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int Age { get; set; }

    public Gender Gender { get; set; }
}
