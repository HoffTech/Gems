// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;
using Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;
using Gems.Patterns.SyncTables.Options;
using Gems.Utils;

using Microsoft.Extensions.Options;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Repository;

public class DestinationSyncedInfoUpdater
{
    private readonly IUnitOfWorkProvider unitOfWorkProvider;
    private readonly IOptions<ChangeTrackingSyncOptions> options;

    public DestinationSyncedInfoUpdater(IUnitOfWorkProvider unitOfWorkProvider, IOptions<ChangeTrackingSyncOptions> options)
    {
        this.unitOfWorkProvider = unitOfWorkProvider;
        this.options = options;
    }

    public Task UpsertChangeTrackingInfoForTableAsync(
        string sourceDbKey,
        string tableName,
        SyncedInfo syncedInfo,
        CancellationToken cancellationToken)
    {
        if (syncedInfo.TableName != tableName)
        {
            throw new ArgumentException(nameof(syncedInfo));
        }

        if (syncedInfo.LastRestoreDateTime.HasValue)
        {
            syncedInfo.LastRestoreDateTime = DateTimeUtils.ConvertToUtc(syncedInfo.LastRestoreDateTime.Value);
        }

        return this.unitOfWorkProvider
            .GetUnitOfWork(sourceDbKey, cancellationToken)
            .CallStoredProcedureAsync(
                this.options.Value.UpsertVersionFunctionInfo.FunctionName,
                new Dictionary<string, object>
                {
                    [this.options.Value.UpsertVersionFunctionInfo.RowVersionParameterName] = syncedInfo
                });
    }
}
