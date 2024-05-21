// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Behaviors;
using Gems.Data.Sample.Context.Persons.UpdatePerson.Entities;

using MediatR;

namespace Gems.Data.Sample.Context.Persons.UpdatePerson.UpdatePersonInternal
{
    public class UpdatePersonInternalCommand : IRequest, IRequestUnitOfWork
    {
        public string UpdatedBy { get; set; }

        public Person Person { get; set; }
    }
}
