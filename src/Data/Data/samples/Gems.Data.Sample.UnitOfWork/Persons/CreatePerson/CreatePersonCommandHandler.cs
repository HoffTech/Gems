// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Data.Sample.UnitOfWork.Persons.CreatePerson
{
    [Endpoint("api/v1/persons/random", "POST", OperationGroup = "Persons")]
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public CreatePersonCommandHandler(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public Task Handle(CreatePersonCommand command, CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallStoredProcedureAsync("public.person_create_random");
        }
    }
}
