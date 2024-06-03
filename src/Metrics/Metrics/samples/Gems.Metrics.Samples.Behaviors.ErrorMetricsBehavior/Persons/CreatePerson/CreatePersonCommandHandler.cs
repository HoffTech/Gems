// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using Gems.Metrics.Samples.Behaviors.ErrorMetricsBehavior.Persons.CreatePerson.BusinessRules;
using Gems.Metrics.Samples.Behaviors.ErrorMetricsBehavior.Persons.CreatePerson.Dto;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Metrics.Samples.Behaviors.ErrorMetricsBehavior.Persons.CreatePerson
{
    [Endpoint("api/v1/persons/create", "POST", OperationGroup = "Persons", Summary = "Создание персоны")]
    public class CreatePersonCommandHandler(BusinessRuleChecker businessRuleChecker)
        : IRequestHandler<CreatePersonCommand, PersonDto>
    {
        public Task<PersonDto> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
        {
            // Проверка возраста персоны
            businessRuleChecker.EnsurePersonIsAdult(command.Age);

            // Создание персоны и сохраниние в БД
            // ...
            return Task.FromResult(
                new PersonDto
                {
                    PersonId = Guid.NewGuid(),
                    Age = command.Age,
                    Gender = command.Gender,
                    FirstName = command.FirstName,
                    LastName = command.LastName
                });
        }
    }
}
