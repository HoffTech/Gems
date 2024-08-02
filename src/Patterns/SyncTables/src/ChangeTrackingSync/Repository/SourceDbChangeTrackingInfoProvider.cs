// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Repository;

public class SourceDbChangeTrackingInfoProvider
{
    private readonly IUnitOfWorkProvider unitOfWorkProvider;

    public SourceDbChangeTrackingInfoProvider(IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.unitOfWorkProvider = unitOfWorkProvider;
    }

    public Task<ChangeTrackingInfo> GetCurrentChangeTrackingInfoAsync(
        string sourceDbKey,
        string tableName,
        string primaryKeyName,
        CancellationToken cancellationToken)
    {
        return this.unitOfWorkProvider
            .GetUnitOfWork(sourceDbKey, cancellationToken)
            .QueryFirstOrDefaultAsync<ChangeTrackingInfo>(
@$"select
	CHANGE_TRACKING_MIN_VALID_VERSION(OBJECT_ID('{tableName}')) MinValidVersion,
	CHANGE_TRACKING_CURRENT_VERSION() CurrentVersion,
	(
	    select max({primaryKeyName}) from {tableName}
	) as MaxKey,
	(
	    select min({primaryKeyName}) from {tableName}
	) as MinKey,
	(
		select top 1 [restore_date]
		from msdb.dbo.[restorehistory] r
		where r.destination_database_name = DB_NAME()
		order by restore_history_id desc
	) as LastRestoreDateTime");
    }
}
