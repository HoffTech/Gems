// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Handlers.Consts;
using Gems.Jobs.Quartz.Jobs.JobTriggerFromDb;
using Gems.Jobs.Quartz.Jobs.JobWithData;
using Gems.Text.Json;

using Quartz;
using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz;

public class TriggerHelper
{
    private readonly IEnumerable<ITriggerDataProvider> triggerDataProviderCollection;

    public TriggerHelper(IEnumerable<ITriggerDataProvider> triggerDataProviderCollection)
    {
        this.triggerDataProviderCollection = triggerDataProviderCollection;
    }

    public static CronTriggerImpl CreateCronTrigger(
        string triggerName,
        string triggerGroup,
        string jobName,
        string jobGroup,
        string cronExp,
        Dictionary<string, object> triggerData = null)
    {
        var newTrigger = new CronTriggerImpl(triggerName, triggerGroup, jobName, jobGroup, cronExp);
        newTrigger.Description = jobName;
        if (triggerData != null && triggerData.Any())
        {
            newTrigger.JobDataMap = new JobDataMap { [QuartzJobWithDataConstants.JobDataKeyValue] = triggerData.Serialize() };
        }

        return newTrigger;
    }

    public ITriggerDataProvider GetTriggerDbType(string providerTypeName)
    {
        return this.triggerDataProviderCollection.FirstOrDefault(curTriggerDataProvider => curTriggerDataProvider.GetType().Name == providerTypeName);
    }

    public async Task<CronTriggerImpl> GetTriggerFromDb(
        string jobName,
        string jobGroup,
        string cronExpression,
        TriggersFromDbOptions triggerFromDb,
        CancellationToken cancellationToken)
    {
        var triggerProviderType = this.GetTriggerDbType(triggerFromDb.ProviderType) ?? throw new InvalidOperationException(
            $"Для триггера {triggerFromDb.TriggerName}, тип {triggerFromDb.ProviderType} не был найден или не реализует интерфейс ITriggerDataProvider");

        var triggerDataDict = await triggerProviderType.GetTriggerData(triggerFromDb.TriggerName, cancellationToken).ConfigureAwait(false);

        var trigger = CreateCronTrigger(
            triggerFromDb.TriggerName ?? jobName,
            jobGroup ?? JobGroups.DefaultGroup,
            jobName,
            jobGroup ?? JobGroups.DefaultGroup,
            cronExpression,
            triggerDataDict);

        return trigger;
    }
}
