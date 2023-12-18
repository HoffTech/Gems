// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Gems.Jobs.Quartz.Consts;

namespace Gems.Jobs.Quartz.Configuration;

public class TriggerOptions
{
    public string Type { get; set; } = TriggerDataType.JobDataType;

    public string CronExpression { get; set; }

    public Dictionary<string, object> JobData { get; set; }

    public string ProviderType { get; set; }
}
