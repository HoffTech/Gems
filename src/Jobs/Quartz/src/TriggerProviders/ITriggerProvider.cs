// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Quartz.Impl.Triggers;

namespace Gems.Jobs.Quartz.TriggerProviders;

public interface ITriggerProvider
{
    Task<List<CronTriggerImpl>> GetTriggers(string jobName, CancellationToken cancellationToken);
}
