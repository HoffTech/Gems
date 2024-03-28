// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Data.Sample.EFCore.Persons.Entities;
using Gems.Data.Sample.EFCore.Persons.GetPersonById.Dto;
using Gems.Data.UnitOfWork.EntityFramework;
using Gems.Mvc.GenericControllers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Gems.Data.Sample.EFCore.Persons.GetPersonById
{
    [Endpoint("api/v1/persons/{id}", "GET", OperationGroup = "Persons")]
    public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, PersonDto>
    {
        private readonly IDbContextProvider dbContextProvider;
        private readonly IMapper mapper;

        public GetPersonByIdQueryHandler(IDbContextProvider dbContextProvider, IMapper mapper)
        {
            this.dbContextProvider = dbContextProvider;
            this.mapper = mapper;
        }

        public async Task<PersonDto> Handle(GetPersonByIdQuery query, CancellationToken cancellationToken)
        {
            var dbContext = await this.dbContextProvider
                .GetDbContext<ApplicationDbContext>(cancellationToken)
                .ConfigureAwait(false);

            return this.mapper.Map<PersonDto>(
                await dbContext.Set<Person>().FirstOrDefaultAsync(p => p.PersonId == query.Id, cancellationToken));
        }
    }
}
