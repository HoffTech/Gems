// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.Npgsql;

using NpgsqlTypes;

namespace Gems.Mvc.Sample.HandlersUsing.Persons.CreatePerson.Entities;

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

    [PgName("__ignore__")]
    public Gender Gender { get; set; }

    [PgName("gender")]
    public int GenderAsInt
    {
        get => (int)this.Gender;
        set => this.Gender = (Gender)Enum.ToObject(typeof(Gender), value);
    }
}
