// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;
using Gems.Patterns.SyncTables.MergeProcessor;
using Gems.Patterns.SyncTables.MergeProcessor.MergeInfos;
using Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons.EntitiesViews;
using Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons.Options;

using MediatR;

using Microsoft.Extensions.Options;

namespace Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons;

[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons")]
public class SyncPersonsCommandHandler : IRequestHandler<SyncPersonsCommand, List<RowCounters>>
{
    private readonly ChangeTrackingMergeProcessorFactory processorFactory;

    private readonly IOptions<SyncPersonsInfoOptions> options;

    public SyncPersonsCommandHandler(
        ChangeTrackingMergeProcessorFactory processorFactory,
        IOptions<SyncPersonsInfoOptions> options)
    {
        this.processorFactory = processorFactory;
        this.options = options;
    }

    public Task<List<RowCounters>> Handle(SyncPersonsCommand request, CancellationToken cancellationToken)
    {
        return new MergeCollection<RowCounters>(
            new List<BaseMergeProcessor<RowCounters>>
            {
                this.processorFactory.CreateChangeTrackingMergeProcessor<ExternalPerson, Person, RowCounters>(
                    new ChangeTrackingMergeInfo<RowCounters>(
                        sourceDbKey: "Dax",
                        tableName: "Persons",
                        externalSyncQuery: "SELECT * FROM Persons",
                        mergeFunctionName: "public.persons_mergepersons",
                        mergeParameterName: "p_persons",
                        needConvertDateTimeToUtc: true,
                        getCommandTimeout: this.options.Value.GetPersonsInfoTimeout,
                        targetDbKey: "DefaultConnection",
                        SyncPersonsMetricType.GetExternalPersonEntities))
            }).ProcessCollectionAsync(cancellationToken);
    }
}
