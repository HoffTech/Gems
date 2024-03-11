// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Data.Npgsql;

using NpgsqlTypes;

namespace Gems.Data.Sample.Transaction.Persons.UpdatePerson.Entities;

[PgType("public.person_type")]
public class Person
{
    [PgName("person_id")]
    public Guid PersonId { get; init; }

    [PgName("first_name")]
    public string FirstName { get; init; }

    [PgName("last_name")]
    public string LastName { get; init; }

    [PgName("age")]
    public int Age { get; init; }

    [PgName("gender")]
    public int Gender { get; init; }
}
