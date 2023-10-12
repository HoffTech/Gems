// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Context;
using Gems.Data.UnitOfWork;
using Gems.Data.UnitOfWork.EntityFramework;

using MediatR;

namespace Gems.Data.Behaviors
{
    public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IEfUnitOfWorkProvider efUnitOfWorkProvider;
        private readonly IContextFactory contextFactory;
        private readonly IContextAccessor contextAccessor;

        public UnitOfWorkBehavior(
            IUnitOfWorkProvider unitOfWorkProvider,
            IEfUnitOfWorkProvider efUnitOfWorkProvider,
            IContextFactory contextFactory,
            IContextAccessor contextAccessor)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.efUnitOfWorkProvider = efUnitOfWorkProvider;
            this.contextFactory = contextFactory;
            this.contextAccessor = contextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is IRequestUnitOfWork r && r.IsSkip())
            {
                return await next();
            }

            // при регистрации unit of work можно указать UnitOfWorkOptions.SuspendTransaction = true. Тогда needTransaction будет игнорироваться.
            var needTransaction = request is IRequestUnitOfWork;
            if (needTransaction)
            {
                this.contextFactory.Create();
            }

            if (this.contextAccessor.Context == null)
            {
                this.contextFactory.Create();
            }

            // если unit of work уже создавался, то оставляем дальнейшее управление тому коду, который и создавал данный unit of work.
            // иначе может получиться, что unit of work может закомититься  и задиспозится раньше времени. Н-р: в пайплайне внутренней команды.
            if (this.unitOfWorkProvider.CheckUnitOfWork(cancellationToken))
            {
                return await next();
            }

            TResponse result;
            try
            {
                var unitOfWorks = this.unitOfWorkProvider.GetUnitOfWorks(needTransaction, cancellationToken);
                var efUnitOfWorks = this.efUnitOfWorkProvider.GetUnitOfWorks(needTransaction, cancellationToken);
                result = await next();
                foreach (var unitOfWork in unitOfWorks)
                {
                    // каммит можно вызывать, даже если needTransaction равен false. В таком случае операция просто проигнорируется.
                    await unitOfWork.CommitAsync();
                }

                foreach (var efUnitOfWork in efUnitOfWorks)
                {
                    // каммит можно вызывать, даже если needTransaction равен false. В таком случае операция просто проигнорируется.
                    await efUnitOfWork.CommitAsync();
                }
            }
            finally
            {
                // данный метод можно вызывать сколько угодно, ошибки не будет. При удалении вызывается Dispose.
                await this.unitOfWorkProvider.RemoveUnitOfWorksAsync(cancellationToken);

                // данный метод можно вызывать сколько угодно, ошибки не будет. При удалении вызывается Dispose.
                await this.efUnitOfWorkProvider.RemoveUnitOfWorksAsync();

                await this.contextFactory.DisposeAsync();
            }

            return result;
        }
    }
}
