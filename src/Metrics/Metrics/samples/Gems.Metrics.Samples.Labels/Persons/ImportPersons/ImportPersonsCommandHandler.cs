// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Mvc.GenericControllers;

using MediatR;

namespace Gems.Metrics.Samples.Labels.Persons.ImportPersons
{
    [Endpoint("api/v1/persons/import", "POST", OperationGroup = "Persons", Summary = "Импорт персон")]
    public class ImportPersonsCommandHandler(IMetricsService metricsService)
        : IRequestHandler<ImportPersonsCommand>
    {
        public async Task Handle(ImportPersonsCommand command, CancellationToken cancellationToken)
        {
            // Получение количества добавленных, обновленных и удаленных персон после выполнения функции импорта
            var counters = ImportPersons();

            await metricsService
                .GaugeSet(ImportPersonsMetricType.ImportPersonCounters, counters.Added, labelValues: "add")
                .ConfigureAwait(false);

            await metricsService
                .GaugeSet(ImportPersonsMetricType.ImportPersonCounters, counters.Updated, labelValues: "update")
                .ConfigureAwait(false);

            await metricsService
                .GaugeSet(ImportPersonsMetricType.ImportPersonCounters, counters.Deleted, labelValues: "delete")
                .ConfigureAwait(false);
        }

        private static PersonCounters ImportPersons()
        {
            // операция импорта персон в БД и получение количества добавленных, обновленных или удаленных персон
            // ...

            // Получение количества добавленных, обновленных и удаленных персон
            return new PersonCounters
            {
                Added = 100,
                Updated = 200,
                Deleted = 300
            };
        }
    }
}
