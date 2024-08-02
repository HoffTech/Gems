// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.GenericControllers;
using Gems.Patterns.SyncTables.ChangeTrackingSync;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Settings;

using Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons.EntitiesViews;
using Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons.Options;

using MediatR;

using Microsoft.Extensions.Options;

namespace Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons;

[Endpoint("api/v1/sync-persons", "POST", OperationGroup = "Persons")]
public class SyncPersonsCommandHandler(
    IChangeTrackingSyncTableProcessor<ExternalPerson, Person, RowCounters> changeTrackingProcessor,
    IOptions<SyncPersonsInfoOptions> options)
    : IRequestHandler<SyncPersonsCommand, List<RowCounters>>
{
    public async Task<List<RowCounters>> Handle(SyncPersonsCommand request, CancellationToken cancellationToken)
    {
        var syncResult = await changeTrackingProcessor.Sync(
            new ChangeTrackingSyncInfo(
                new SourceDataSettings
                {
                    DbKey = "source",
                    TableName = "dbo.Person",
                    GetCommandTimeout = options.Value.GetPersonsInfoTimeout,
                    BatchSize = 100_000,
                    PrimaryKeyName = "RecId",
                    ChangesQuery =
                        """
                        WITH ChangedPersonCTE
                        (
                            [ChangeTrackingVersion],
                            [OperationType],
                            [RecId],
                            [PersonId],
                            [FirstName],
                            [LastName],
                            [Age],
                            [Gender]
                        )
                        AS
                        (
                            SELECT
                                ct.SYS_CHANGE_VERSION [ChangeTrackingVersion],
                                ct.SYS_CHANGE_OPERATION [OperationType],
                                ct.[RecId],
                                [PersonId],
                                [FirstName],
                                [LastName],
                                [Age],
                                [Gender]
                            FROM changetable(changes dbo.[Person], @version) as ct
                            LEFT JOIN dbo.[Person] as it
                                on ct.RECID = it.RECID
                        )

                        SELECT TOP (@batchSize) WITH TIES
                            *
                        FROM  ChangedPersonCTE WITH (FORCESEEK)
                        ORDER BY ChangeTrackingVersion
                        OPTION(MAXDOP 1)
                        """,
                    FullReloadQuery =
                        """
                        select
                            0 [ChangeTrackingVersion],
                            'I' [OperationType],
                            [RecId],
                            [PersonId],
                            [FirstName],
                            [LastName],
                            [Age],
                            [Gender]
                        from dbo.[Person]
                        where
                            [RecId] >= @offset and [RecId] < @offset + @batchSize
                        order by [RecId]
                        """,
                    OnRestoreFromBackupDetected = SyncErrorAction.Fail,
                    OnDestinationVersionOutdated = SyncErrorAction.Fail
                },
                new DestinationSettings
                {
                    DbKey = "destination",
                    TableName = "public.person",
                    MergeFunctionName = "public.person_merge",
                    MergeParameterName = "p_changed_data",
                    EnableFullChangesLog = false,
                    ClearFunctionName = "public.person_clear"
                },
                true),
            cancellationToken);

        return syncResult.MergeResults.ToList();
    }
}
