// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Metrics.Samples.Operations.Counter.Persons.CreatePerson.Dto;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Metrics.Samples.Operations.Counter.Persons.CreatePerson
{
    [Endpoint("api/v1/persons/create", "POST", OperationGroup = "Persons", Summary = "Создание персоны")]
    public class CreatePersonCommandHandler(IMetricsService metricsService)
        : IRequestHandler<CreatePersonCommand, PersonDto>
    {
        public async Task<PersonDto> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
        {
            // Создание персоны и сохраниние в БД
            // ...

            // Счетчик количества созданных персон с момента старта сервиса. Инкремент на 1
            await metricsService.Counter(CreatePersonMetricType.PersonsCreated).ConfigureAwait(false);

            return new PersonDto
            {
                PersonId = Guid.NewGuid(),
                Age = command.Age,
                Gender = command.Gender,
                FirstName = command.FirstName,
                LastName = command.LastName
            };
        }
    }
}
