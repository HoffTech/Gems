// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Jobs.Quartz.Jobs;

public interface IHasTriggerData
{
    public Dictionary<string, object> JobData { get; set; }
}
