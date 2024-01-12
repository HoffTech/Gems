// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Jobs.Quartz.Configuration;

public class TriggersWithDataOptions
{
    public string TriggerName { get; set; }

    public string CronExpression { get; set; }

    public Dictionary<string, object> TriggerData { get; set; }
}
