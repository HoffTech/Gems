// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Quartz;

namespace Gems.Jobs.Quartz.Handlers.GetListOfJobs.Dto;

public class TriggerInfo
{
    public ITrigger Trigger { get; set; }

    public string CronExpression { get; set; }

    public string TriggerState { get; set; }
}
