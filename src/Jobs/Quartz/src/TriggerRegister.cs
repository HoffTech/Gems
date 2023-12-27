// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Gems.Jobs.Quartz.Configuration;
using Gems.Jobs.Quartz.Consts;

namespace Gems.Jobs.Quartz;

public static class TriggerRegister
{
    public static async Task<List<TriggerOptions>> GetTriggerData(object triggerData)
    {
        var triggerOptionsList = new List<TriggerOptions>();
        switch (triggerData)
        {
            case string triggerCronExpr:
                triggerOptionsList.Add(new TriggerOptions { CronExpression = triggerCronExpr });
                break;
            case List<TriggerOptions> triggerOptions:
                foreach (var triggerOption in triggerOptions.Where(o => o.Type == TriggerDataType.ConfigType))
                {
                    if (triggerOption.Type == TriggerDataType.ConfigType)
                    {
                        triggerOptionsList.Add(
                            new TriggerOptions
                            {
                                CronExpression = triggerOption.CronExpression,
                                TriggerData = triggerOption.TriggerData,
                                TriggerName = triggerOption.TriggerName
                            });
                    }
                    else if (triggerOption.Type == TriggerDataType.DataBaseType && Type.GetType(triggerOption.ProviderType)?.GetInterface(nameof(ITriggerDataProvider)) != null)
                    {
                        var cronExpression = await (Type.GetType(triggerOption.ProviderType) as ITriggerDataProvider).GetCronExpression().ConfigureAwait(false);
                        var triggerDataDict = await (Type.GetType(triggerOption.ProviderType) as ITriggerDataProvider).GetTriggerData().ConfigureAwait(false);
                        triggerOptionsList.Add(
                            new TriggerOptions
                            {
                                CronExpression = cronExpression,
                                TriggerData = triggerDataDict,
                                TriggerName = triggerOption.TriggerName
                            });
                    }
                }

                break;
        }

        return triggerOptionsList;
    }
}
