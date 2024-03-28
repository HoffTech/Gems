// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Gems.Data.Sample.Context.Persons.UpdatePerson.Entities;
using Gems.Data.Sample.Context.Persons.UpdatePerson.UpdatePersonInternal;
using Gems.Mvc.Filters.Exceptions;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Data.Sample.Context.Persons.UpdatePerson
{
    [Endpoint("api/v1/persons/update", "POST", OperationGroup = "Persons")]
    public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonsCommand>
    {
        private readonly SessionRepository sessionRepository;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public UpdatePersonCommandHandler(SessionRepository sessionRepository, IMapper mapper, IMediator mediator)
        {
            this.sessionRepository = sessionRepository;
            this.mapper = mapper;
            this.mediator = mediator;
        }

        public async Task Handle(UpdatePersonsCommand command, CancellationToken cancellationToken)
        {
            // 1. Сначала производим проверку на текущую сессию (создаем новую, обновляем количество запросов или сбрасываем)
            // Эту операцию необходимо выполнить прежде всего отдельно от транзакции, чтобы вести счетик запросов
            await this.ProcessSessionAsync(command, cancellationToken);

            // 2. Затем в отдельной транзакции обновляем объект Person и объект Log
            await this.mediator
                .Send(
                    new UpdatePersonInternalCommand
                    {
                        Person = this.mapper.Map<Person>(command.Person),
                        UpdatedBy = command.UpdatedBy
                    },
                    cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task ProcessSessionAsync(UpdatePersonsCommand command, CancellationToken cancellationToken)
        {
            var session = await this.sessionRepository
                .GetSessionByIdAsync(command.SessionId, cancellationToken)
                .ConfigureAwait(false);

            if (session is null)
            {
                await this.sessionRepository.CreateSessionAsync(command.SessionId, cancellationToken);
            }
            else if (session.SubmittedAt >= DateTime.UtcNow.AddMinutes(-1))
            {
                const int maxRequestsForSession = 3;
                if (session.Quantity > maxRequestsForSession)
                {
                    throw new TooManyRequestsException($"Превышено максимальное количество запросов для сессии {session.SessionId}");
                }

                await this.sessionRepository
                    .UpdateSessionQuantityAsync(command.SessionId, session.Quantity + 1, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await this.sessionRepository
                    .ResetSessionAsync(command.SessionId, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
