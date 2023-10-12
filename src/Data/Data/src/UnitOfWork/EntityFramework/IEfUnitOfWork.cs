// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Gems.Data.UnitOfWork.EntityFramework;

public interface IEfUnitOfWork : IAsyncDisposable
{
    Task<DbContext> GetDbContextAsync();

    Task CommitAsync();
}
