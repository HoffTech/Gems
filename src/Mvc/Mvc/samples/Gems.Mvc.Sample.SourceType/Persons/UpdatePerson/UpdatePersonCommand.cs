// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Behaviors;
using Gems.Mvc.Sample.SourceType.Persons.UpdatePerson.Dto;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Mvc.Sample.SourceType.Persons.UpdatePerson;

public class UpdatePersonCommand : IRequest<Guid>, IRequestUnitOfWork
{
    [FromBody]
    public Person Person { get; set; }
}
