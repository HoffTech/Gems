using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Data.Sample.Operations.Persons.Shared.Entities;
using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Data.Sample.Operations.Persons.CreatePerson
{
    [Endpoint("api/v1/person", "POST", OperationGroup = "Persons")]
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IMapper mapper;

        public CreatePersonCommandHandler(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.mapper = mapper;
        }

        public Task Handle(CreatePersonCommand command, CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallStoredProcedureAsync(
                    "public.person_create",
                    new Dictionary<string, object>
                    {
                        ["p_person"] = this.mapper.Map<Person>(command.Person)
                    });
        }
    }
}
