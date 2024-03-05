// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Patterns.SyncTables.EntitiesViews;

namespace Gems.Patterns.SyncTables.Sample.SyncTables.Persons.SyncPersons.EntitiesViews;

public class ExternalPerson : ExternalChangeTrackingEntity
{
    public Guid PersonId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int Age { get; set; }

    public Gender Gender { get; set; }
}
