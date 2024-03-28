// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Data.Sample.Metrics.Persons.GetPerson.Dto;
using Gems.Data.Sample.Metrics.Persons.GetPerson.Entities;
using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Data.Sample.Metrics.Persons.GetPerson
{
    [Endpoint("api/v1/persons/{id}", "GET", OperationGroup = "Persons")]
    public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, PersonDto>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IMapper mapper;

        public GetPersonQueryHandler(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.mapper = mapper;
        }

        public async Task<PersonDto> Handle(GetPersonQuery query, CancellationToken cancellationToken)
        {
            var person = await this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallTableFunctionFirstAsync<Person>(
                    "public.person_get_person_by_id",
                    new Dictionary<string, object>
                    {
                        ["p_person_id"] = query.Id
                    });

            return this.mapper.Map<PersonDto>(person);
        }
    }
}
