// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Data.Sample.Operations.Persons.GetPersonAge
{
    [Endpoint("api/v1/persons/{id}/age", "GET", OperationGroup = "Persons")]
    public class GetPersonAgeQueryHandler : IRequestHandler<GetPersonAgeQuery, int>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public GetPersonAgeQueryHandler(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public Task<int> Handle(GetPersonAgeQuery query, CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallScalarFunctionAsync<int>(
                    "public.person_get_age_by_id",
                    new Dictionary<string, object>
                    {
                        ["p_person_id"] = query.Id
                    });
        }
    }
}
