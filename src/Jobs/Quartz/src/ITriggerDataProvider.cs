// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gems.Jobs.Quartz;

public interface ITriggerDataProvider
{
   Task<string> GetCronExpression(string triggerName, CancellationToken cancellationToken);

   Task<Dictionary<string, object>> GetTriggerData(string triggerName, CancellationToken cancellationToken);
}
