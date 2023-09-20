// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Quartz;

namespace Gems.Jobs.Quartz.Handlers.GetListOfJobs.Dto;

public class JobInfo
{
    public JobKey JobKey { get; set; }

    public List<TriggerInfo> Triggers { get; set; }
}
