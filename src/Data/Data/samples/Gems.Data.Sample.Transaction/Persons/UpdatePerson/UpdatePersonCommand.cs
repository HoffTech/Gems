// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Behaviors;
using Gems.Data.Sample.Transaction.Persons.UpdatePerson.Dto;

using MediatR;

namespace Gems.Data.Sample.Transaction.Persons.UpdatePerson
{
    public class UpdatePersonCommand : IRequest, IRequestUnitOfWork
    {
        public string UpdatedBy { get; init; }

        public PersonDto Person { get; init; }
    }
}
