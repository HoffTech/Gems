// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Metrics.Samples.Operations.Time.Persons.CreatePerson.Dto;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Metrics.Samples.Operations.Time.Persons.CreatePerson
{
    [Endpoint("api/v1/persons/create", "POST", OperationGroup = "Persons", Summary = "Создание персоны")]
    public class CreatePersonCommandHandler(IMetricsService metricsService)
        : IRequestHandler<CreatePersonCommand, PersonDto>
    {
        public async Task<PersonDto> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
        {
            // Создание персоны и сохраниние в БД
            // ...

            // замер длительности регистрации персоны
            return await this
                .RegisterPersonAsync(
                    new PersonDto
                    {
                        PersonId = Guid.NewGuid(),
                        Age = command.Age,
                        Gender = command.Gender,
                        FirstName = command.FirstName,
                        LastName = command.LastName
                    },
                    cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<PersonDto> RegisterPersonAsync(PersonDto person, CancellationToken cancellationToken)
        {
            // установка временной метрики на длительность процесса регистрации персоны
            await using var timeMetric = metricsService
                .Time(CreatePersonMetricType.PersonRegistrationTime)
                .ConfigureAwait(false);

            // мнимый процесс регистрации
            await Task.Delay(1500, cancellationToken).ConfigureAwait(false);

            return person;
        }
    }
}
