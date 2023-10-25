// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Jobs.Quartz.Configuration;

public class BlockedJobsRecoveryOptions
{
    /// <summary>
    /// Список worker'ов, для которых нужно пересоздавать Job'ы и Trigger'ы, если они находятся в Blocked состоянии в течение <seealso cref="JobsOptions.JobRecoveryDelayInMilliseconds" />
    /// </summary>
    public List<string> WorkersToRecover { get; set; }

    /// <summary>
    /// Задержка перед итерацией мониторинга и восстановления триггеров, находящихся в состоянии Blocked.
    /// </summary>
    public int CheckIntervalInMilliseconds { get; set; } = 15 * 60 * 1000;

    /// <summary>
    /// Задержка между временем последнего запуска воркера и текущим временем
    /// </summary>
    public int MaxDelayBetweenLastFireTimeAndRecoverTimeInMilliseconds { get; set; } = 5 * 60 * 1000;
}
