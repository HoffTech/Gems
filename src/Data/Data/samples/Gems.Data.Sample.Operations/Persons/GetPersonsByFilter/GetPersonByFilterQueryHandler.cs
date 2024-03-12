// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Data.Sample.Operations.Persons.GetPersonsByFilter.Dto;
using Gems.Data.Sample.Operations.Persons.Shared.Entities;
using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Data.Sample.Operations.Persons.GetPersonsByFilter
{
    [Endpoint("api/v1/persons/by-filter", "GET", OperationGroup = "Persons")]
    public class GetPersonByFilterQueryHandler : IRequestHandler<GetPersonByFilterQuery, List<PersonDto>>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IMapper mapper;

        public GetPersonByFilterQueryHandler(IUnitOfWorkProvider unitOfWorkProvider, IMapper mapper)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.mapper = mapper;
        }

        public async Task<List<PersonDto>> Handle(GetPersonByFilterQuery query, CancellationToken cancellationToken)
        {
            var person = await this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .QueryFirstOrDefaultAsync<Person>(CreateQuery(query, out var parameters), parameters);

            return this.mapper.Map<List<PersonDto>>(person);
        }

        private static string CreateQuery(GetPersonByFilterQuery query, out Dictionary<string, object> parameters)
        {
            const string sql = """
                               SELECT * FROM public.person p
                               WHERE TRUE
                               """;

            var sb = new StringBuilder(sql);
            parameters = new Dictionary<string, object>();
            if (query.Age is not null)
            {
                sb.AppendLine("AND p.age = @Age");
                parameters.Add("@Age", query.Age);
            }

            if (string.IsNullOrEmpty(query.FirstName))
            {
                sb.AppendLine("AND p.first_name = @FirstName");
                parameters.Add("@FirstName", query.FirstName);
            }

            if (string.IsNullOrEmpty(query.LastName))
            {
                sb.AppendLine("AND p.last_name = @LastName");
                parameters.Add("@LastName", query.FirstName);
            }

            sb.AppendLine("LIMIT @Take OFFSET @Skip");
            parameters.Add("@Take", query.Take ?? 100);
            parameters.Add("@SKIP", query.Skip ?? 0);

            return sb.ToString();
        }
    }
}
