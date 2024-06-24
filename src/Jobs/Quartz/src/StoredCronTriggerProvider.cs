// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;

namespace Gems.Jobs.Quartz;

public class StoredCronTriggerProvider
{
    private readonly IUnitOfWorkProvider unitOfWorkProvider;

    public StoredCronTriggerProvider(IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.unitOfWorkProvider = unitOfWorkProvider;
    }

    public Task<string> GetCronExpression(string triggerName, CancellationToken cancellationToken = default)
    {
        return this.unitOfWorkProvider.GetUnitOfWork(cancellationToken)
            .CallScalarFunctionAsync<string>(
                "quartz.get_qrtz_stored_cron_triggers",
                new Dictionary<string, object>
                {
                    { "p_trigger_name", triggerName },
                });
    }

    public Task WriteCronExpression(
        string triggerName,
        string cronExpression,
        CancellationToken cancellationToken = default)
    {
        return this.unitOfWorkProvider.GetUnitOfWork(cancellationToken)
            .CallStoredProcedureAsync(
                "quartz.upsert_qrtz_stored_cron_triggers",
                new Dictionary<string, object>
                {
                    { "p_trigger_name", triggerName },
                    { "p_cron_expression", cronExpression },
                });
    }

    public Task DeleteCronExpression(
        string triggerName,
        CancellationToken cancellationToken = default)
    {
        return this.unitOfWorkProvider.GetUnitOfWork(cancellationToken)
            .CallStoredProcedureAsync(
                "quartz.delete_qrtz_stored_cron_triggers",
                new Dictionary<string, object>
                {
                    { "p_trigger_name", triggerName },
                });
    }
}
