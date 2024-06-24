// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz.TriggerValidators;

public interface ITriggerValidator
{
    bool CheckIsValid(IEnumerable<CronTriggerImpl> triggerCollection, out string errorMessage);
}
