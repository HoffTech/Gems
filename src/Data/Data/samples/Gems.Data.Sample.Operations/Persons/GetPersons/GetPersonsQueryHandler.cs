// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Data.Sample.Operations.Persons.GetPersons.Dto;
using Gems.Data.Sample.Operations.Persons.Shared.Entities;
using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Data.Sample.Operations.Persons.GetPersons
{
    [Endpoint("api/v1/persons", "GET", OperationGroup = "Persons")]
    public class GetPersonsQueryHandler : IRequestHandler<GetPersonsQuery, List<PersonDto>>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IMapper mapper;

        public GetPersonsQueryHandler(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.mapper = mapper;
        }

        public async Task<List<PersonDto>> Handle(GetPersonsQuery query, CancellationToken cancellationToken)
        {
            var persons = await this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallTableFunctionAsync<Person>(
                    "public.person_get_persons",
                    new Dictionary<string, object>
                    {
                        ["p_skip"] = query.Skip ?? default,
                        ["p_take"] = query.Take ?? 100
                    });

            return this.mapper.Map<List<PersonDto>>(persons);
        }
    }
}
