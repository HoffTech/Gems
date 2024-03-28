// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gems.Data.Sample.EFCore.Persons.Entities;

[Table("person", Schema = "public")]
public class Person
{
    [Key]
    [Required]
    [Column("person_id")]
    public Guid PersonId { get; init; }

    [Required]
    [Column("first_name")]
    public string FirstName { get; init; }

    [Required]
    [Column("last_name")]
    public string LastName { get; init; }

    [Required]
    [Column("age")]
    public int Age { get; init; }

    [Required]
    [Column("gender")]
    public int Gender { get; init; }
}
