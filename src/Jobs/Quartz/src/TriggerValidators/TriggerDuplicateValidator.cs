// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz.TriggerValidators;

public class TriggerDuplicateValidator : ITriggerValidator
{
    public bool CheckIsValid(IEnumerable<CronTriggerImpl> triggerCollection, out string errorMessage)
    {
        var cronTriggerImpls = triggerCollection as CronTriggerImpl[] ?? triggerCollection.ToArray();

        var result = cronTriggerImpls.DistinctBy(c => c.Name).Count() == cronTriggerImpls.Length;

        errorMessage = result is false ? "Обнаружены тригегры-дубликаты" : null;

        return result;
    }
}
