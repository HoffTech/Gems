// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Data.Sample.Transaction.Persons.UpdatePerson.Entities;
using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Data.Sample.Transaction.Persons.UpdatePerson
{
    [Endpoint("api/v1/persons/update", "POST", OperationGroup = "Persons")]
    public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IMapper mapper;

        public UpdatePersonCommandHandler(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.mapper = mapper;
        }

        public async Task Handle(UpdatePersonCommand command, CancellationToken cancellationToken)
        {
            await this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallStoredProcedureAsync(
                    "public.person_update_person",
                    new Dictionary<string, object>
                    {
                        ["p_person"] = this.mapper.Map<Person>(command.Person)
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
