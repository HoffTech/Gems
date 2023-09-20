// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gems.Data.UnitOfWork
{
    public interface IUnitOfWorkProvider
    {
        IUnitOfWork GetUnitOfWork(string key, bool needTransaction, CancellationToken cancellationToken);

        IUnitOfWork GetUnitOfWork(string key, CancellationToken cancellationToken);

        IUnitOfWork GetUnitOfWork(bool needTransaction, CancellationToken cancellationToken);

        IUnitOfWork GetUnitOfWork(CancellationToken cancellationToken);

        List<IUnitOfWork> GetUnitOfWorks(bool needTransaction, CancellationToken cancellationToken);

        Task RemoveUnitOfWorkAsync(string key, CancellationToken cancellationToken);

        Task RemoveUnitOfWorkAsync(CancellationToken cancellationToken);

        Task RemoveUnitOfWorksAsync(CancellationToken cancellationToken);

        bool CheckUnitOfWork(CancellationToken cancellationToken);
    }
}
