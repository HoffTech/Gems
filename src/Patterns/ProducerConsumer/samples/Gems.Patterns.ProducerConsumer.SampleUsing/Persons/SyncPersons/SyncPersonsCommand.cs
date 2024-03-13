// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Behaviors;
using Gems.Patterns.ProducerConsumer.SampleUsing.Persons.SyncPersons.EntitiesViews;

using MediatR;

namespace Gems.Patterns.ProducerConsumer.SampleUsing.Persons.SyncPersons;

public class SyncPersonsCommand : IRequest, IRequestUnitOfWork
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int Age { get; set; }

    public Gender Gender { get; set; }
}
