// <copyright file="MigrationDbContext.cs" company="Hoff">
// Copyright (c) Company. All rights reserved.
// </copyright>

using System.Reflection;

using Microsoft.EntityFrameworkCore;

namespace Gems.Data.Sample.EFCore.Persons;

public class MigrationDbContext : DbContext
{
    public MigrationDbContext(DbContextOptions<MigrationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
