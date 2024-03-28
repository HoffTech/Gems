// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Data.Behaviors;
using Gems.Data.Sample.Context.Persons.UpdatePerson.Dto;

using MediatR;

namespace Gems.Data.Sample.Context.Persons.UpdatePerson
{
    public class UpdatePersonsCommand : IRequest, IRequestUnitOfWork
    {
        public Guid SessionId { get; set; }

        public string UpdatedBy { get; set; }

        public PersonDto Person { get; init; }
    }
}
