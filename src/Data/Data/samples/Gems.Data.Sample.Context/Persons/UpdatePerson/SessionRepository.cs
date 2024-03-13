using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.Sample.Context.Persons.UpdatePerson.Entities;
using Gems.Data.UnitOfWork;

namespace Gems.Data.Sample.Context.Persons.UpdatePerson
{
    public class SessionRepository
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public SessionRepository(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public Task<Session> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallTableFunctionFirstAsync<Session>(
                    "public.session_get_session_by_id",
                    new Dictionary<string, object>
                    {
                        ["p_session_id"] = sessionId
                    });
        }

        public Task CreateSessionAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallStoredProcedureAsync(
                    "public.session_create",
                    new Dictionary<string, object>
                    {
                        ["p_session_id"] = sessionId,
                        ["p_submitted_at"] = DateTime.UtcNow,
                        ["p_quantity"] = 1
                    });
        }

        public Task UpdateSessionQuantityAsync(Guid sessionId, int quantity, CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallStoredProcedureAsync(
                    "public.session_update_qty",
                    new Dictionary<string, object>
                    {
                        ["p_session_id"] = sessionId,
                        ["p_quantity"] = quantity
                    });
        }

        public async Task ResetSessionAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            await this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallStoredProcedureAsync(
                    "public.session_reset",
                    new Dictionary<string, object>
                    {
                        ["p_session_id"] = sessionId
                    })
                .ConfigureAwait(false);
        }
    }
}
