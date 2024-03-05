// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Patterns.ProducerConsumer.SampleUsing.Persons.Shared.Entities;

namespace Gems.Patterns.ProducerConsumer.SampleUsing.Persons.SyncPersons.EntitiesViews;

public class ExternalPerson
{
    public Guid PersonId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int Age { get; set; }

    public Gender Gender { get; set; }
}
