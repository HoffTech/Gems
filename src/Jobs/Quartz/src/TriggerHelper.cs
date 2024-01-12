// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Jobs.JobWithData;
using Gems.Text.Json;

using Quartz;
using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz;

public class TriggerHelper
{
    public CronTriggerImpl CreateCronTrigger(
        string triggerName,
        string triggerGroup,
        string jobName,
        string jobGroup,
        string cronExp,
        Dictionary<string, object> triggerData)
    {
        var newTrigger = new CronTriggerImpl(triggerName, triggerGroup, jobName, jobGroup, cronExp);
        newTrigger.Description = jobName;
        if (triggerData != null && triggerData.Any())
        {
            newTrigger.JobDataMap = new JobDataMap { [QuartzJobWithDataConstants.JobDataKeyValue] = triggerData.Serialize() };
        }

        return newTrigger;
    }

    public ITriggerDataProvider GetTriggerDbType(TriggersFromDbOptions triggerFromDb)
    {
        var triggerProviderType = Assembly.GetEntryAssembly()?.GetTypes()
            .Where(x => typeof(ITriggerDataProvider).IsAssignableFrom(x) && x.Name.Contains(triggerFromDb.ProviderType))
            .Select(Activator.CreateInstance)
            .Cast<ITriggerDataProvider>()
            .FirstOrDefault();
        if (triggerProviderType == null)
        {
            throw new InvalidOperationException(
                $"Для триггера {triggerFromDb.TriggerName}, тип {triggerFromDb.ProviderType} не был найден или не реализует интерфейс ITriggerDataProvider");
        }

        return triggerProviderType;
    }
}
