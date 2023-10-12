﻿// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Gems.Data.UnitOfWork.EntityFramework;

public interface IDbContextProvider
{
    Task<TDbContext> GetDbContext<TDbContext>(CancellationToken cancellationToken)
        where TDbContext : DbContext;
}
