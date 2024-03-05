// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Data.SqlClient;

using Gems.Data;
using Gems.Mvc.GenericControllers;
using Gems.Patterns.ProducerConsumer.SampleUsing.Persons.SyncPersons.EntitiesViews;

using MediatR;

using Microsoft.Extensions.Options;

namespace Gems.Patterns.ProducerConsumer.SampleUsing.Persons.SyncPersons;

[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons")]
public class SyncPersonsCommandHandler : IRequestHandler<SyncPersonsCommand>
{
    private readonly IOptions<ProducerConsumerOptions> producerConsumerOptions;

    public SyncPersonsCommandHandler(IOptions<ProducerConsumerOptions> producerConsumerOptions)
    {
        this.producerConsumerOptions = producerConsumerOptions;
    }

    public Task Handle(SyncPersonsCommand request, CancellationToken cancellationToken)
    {
        var producerConsumer = new ProducerConsumer<ExternalPerson>(this.producerConsumerOptions, StartProduceAsync, StartConsumerAsync);

        return producerConsumer.StartAsync(cancellationToken);
    }

    private static async Task StartProduceAsync(
        ProducerConsumer<ExternalPerson> producerConsumer,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection("SOME CONNECTION STRING");
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = new SqlCommand("SELECT PersonId FROM dbo.PERSONS", connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        if (!reader.HasRows)
        {
            return;
        }

        var colPersonId = reader.GetOrdinal("PersonId");

        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            producerConsumer.AddTaskInfo(new ExternalPerson
            {
                PersonId = reader.ReadData(colPersonId, reader.GetGuid),
            });
        }
    }

    private static Task StartConsumerAsync(ExternalPerson person, CancellationToken cancellationToken)
    {
        // здесь код записи в целевую базу данных
        throw new NotImplementedException();
    }
}
