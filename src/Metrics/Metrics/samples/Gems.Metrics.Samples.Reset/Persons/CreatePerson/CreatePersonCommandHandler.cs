// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Metrics.Samples.Reset.Persons.CreatePerson.Dto;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Metrics.Samples.Reset.Persons.CreatePerson
{
    [Endpoint("api/v1/persons/create", "POST", OperationGroup = "Persons", Summary = "Создание персоны")]
    public class CreatePersonCommandHandler(IMetricsService metricsService)
        : IRequestHandler<CreatePersonCommand, PersonDto>
    {
        public async Task<PersonDto> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
        {
            // Создание персоны и сохраниние в БД
            // ...

            // Датчик возраста персон с момента старта сервиса
            // По выборке данной метрики можно отслеживать возраст передаваемых персон и вычислить средний возраст
            await metricsService
                .GaugeSet(CreatePersonMetricType.PersonAge, targetValue: command.Age)
                .ConfigureAwait(false);

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
