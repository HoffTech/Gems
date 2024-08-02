// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

namespace Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons.EntitiesViews;

public class ExternalPerson : ISourceChangeTrackingEntity
{
    public long ChangeTrackingVersion { get; set; }

    public string OperationType { get; set; }

    public long RecId { get; set; }

    public Guid PersonId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int Age { get; set; }

    public int Gender { get; set; }
}
