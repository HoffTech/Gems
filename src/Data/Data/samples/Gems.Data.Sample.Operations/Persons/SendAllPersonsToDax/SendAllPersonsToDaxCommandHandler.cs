// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.Sample.Operations.Persons.Shared.Entities;
using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Data.Sample.Operations.Persons.SendAllPersonsToDax
{
    [Endpoint("api/v1/persons/send-to-dax", "POST", OperationGroup = "Persons")]
    public class SendAllPersonsToDaxCommandHandler : IRequestHandler<SendAllPersonsToDaxCommand>
    {
        private const int PersonsBatchSize = 1000;

        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public SendAllPersonsToDaxCommandHandler(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public async Task Handle(SendAllPersonsToDaxCommand query, CancellationToken cancellationToken)
        {
            var personsBuffer = new List<Person>();

            await foreach (var person in this.GetPersonsAsAsyncEnumerable(cancellationToken))
            {
                personsBuffer.Add(person);

                if (personsBuffer.Count >= PersonsBatchSize)
                {
                    await this.SendPersonsToDaxAsync(personsBuffer, cancellationToken).ConfigureAwait(false);
                }
            }

            if (personsBuffer.Count >= 0)
            {
                await this.SendPersonsToDaxAsync(personsBuffer, cancellationToken).ConfigureAwait(false);
            }
        }

        private IAsyncEnumerable<Person> GetPersonsAsAsyncEnumerable(CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .ExecuteReaderAsync<Person>("SELECT * FROM public.person");
        }

        private async Task SendPersonsToDaxAsync(List<Person> persons, CancellationToken cancellationToken)
        {
            // отправка пачки персон в Dax
            Console.WriteLine($"Отправлено персон в Dax: {persons.Count}");
            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
        }
    }
}
