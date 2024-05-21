// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

using MediatR;

namespace Gems.Data.Sample.Context.Persons.UpdatePerson.UpdatePersonInternal
{
    public class UpdatePersonInternalCommandHandler : IRequestHandler<UpdatePersonInternalCommand>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public UpdatePersonInternalCommandHandler(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public async Task Handle(UpdatePersonInternalCommand command, CancellationToken cancellationToken)
        {
            await this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallStoredProcedureAsync(
                    "public.person_update_person",
                    new Dictionary<string, object>
                    {
                        ["p_person"] = command.Person
                    });

            await this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallStoredProcedureAsync(
                    "public.log_update_log",
                    new Dictionary<string, object>
                    {
                        ["p_updated_by"] = command.UpdatedBy
                    });
        }
    }
}
