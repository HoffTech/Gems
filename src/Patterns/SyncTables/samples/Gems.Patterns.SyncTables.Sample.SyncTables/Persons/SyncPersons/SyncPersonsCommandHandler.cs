// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;
using Gems.Patterns.SyncTables.MergeProcessor;
using Gems.Patterns.SyncTables.MergeProcessor.MergeInfos;
using Gems.Patterns.SyncTables.Sample.SyncTables.Persons.SyncPersons.EntitiesViews;

using MediatR;

namespace Gems.Patterns.SyncTables.Sample.SyncTables.Persons.SyncPersons;

[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons")]
public class SyncPersonsCommandHandler : IRequestHandler<SyncPersonsCommand>
{
    private readonly MergeProcessorFactory mergeProcessorFactory;

    public SyncPersonsCommandHandler(MergeProcessorFactory mergeProcessorFactory)
    {
        this.mergeProcessorFactory = mergeProcessorFactory;
    }

    public Task Handle(SyncPersonsCommand request, CancellationToken cancellationToken)
    {
        return new MergeCollection<EmptyMergeResult>(
            new List<BaseMergeProcessor<EmptyMergeResult>>
            {
                this.mergeProcessorFactory.CreateMergeProcessor<ExternalPerson, Person, EmptyMergeResult>(
                    new MergeInfo<EmptyMergeResult>(
                        sourceDbKey: "Dax",
                        externalSyncQuery: "SELECT * FROM Persons",
                        mergeFunctionName: "common.persons_upsert",
                        mergeParameterName: "p_persons",
                        needConvertDateTimeToUtc: false,
                        getCommandTimeout: 0,
                        targetDbKey: "DefaultConnection",
                        externalDbQueryMetricType: SyncPersonsMetricType.GetExternalPersonEntities))
            })
            .ProcessCollectionAsync(cancellationToken);
    }
}
