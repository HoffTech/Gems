﻿// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Data.Npgsql;

using NpgsqlTypes;

namespace Gems.Data.Sample.UnitOfWork.Persons.CreatePerson.Entities;

[PgType("public.person_type")]
public class Person
{
    [PgName("person_id")]
    public Guid PersonId { get; set; }

    [PgName("first_name")]
    public string FirstName { get; set; }

    [PgName("last_name")]
    public string LastName { get; set; }

    [PgName("age")]
    public int Age { get; set; }

    [PgName("gender")]
    public int Gender { get; set; }
}
