// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace Gems.Jobs.Quartz.Configuration
{
    public class JobsOptions
    {
        /// <summary>
        /// Name in appsettings.json.
        /// </summary>
        public const string Jobs = "Jobs";

        /// <summary>
        /// ConnectionString to Quartz db.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Database type.
        /// </summary>
        public QuartzDbType Type { get; set; } = QuartzDbType.PostgreSql;

        /// <summary>
        /// SchedulerName.
        /// </summary>
        public string SchedulerName { get; set; }

        /// <summary>
        /// Dictionary of triggers, where key is trigger's name and value is trigger's cron expression.
        /// </summary>
        public Dictionary<string, string> Triggers { get; set; }

        /// <summary>
        /// Префикс именования таблиц jobStore.
        /// </summary>
        public string TablePrefix { get; set; }

        public QuartzProperties QuartzProperties { get; set; }

        /// <summary>
        /// Количество потоков, доступных для одновременного выполнения заданий в Quartz.
        /// </summary>
        [Obsolete("Использовать QuartzProperties.ThreadPool.MaxConcurrency")]
        public int? MaxConcurrency { get; set; }

        [Obsolete("Использовать QuartzProperties.Scheduler.BatchTriggerAcquisitionMaxCount")]
        public int? BatchTriggerAcquisitionMaxCount { get; set; }

        [Obsolete("Использовать QuartzProperties.Scheduler.AcquireTriggersWithinLock")]
        public bool? AcquireTriggersWithinLock { get; set; }

        /// <summary>
        /// Задержка перед итерацией мониторинга и восстановления триггеров, находящихся в состоянии Error.
        /// </summary>
        public int JobRecoveryDelayInMilliseconds { get; set; } = 15 * 60 * 1000;

        /// <summary>
        /// Подключение/отключение персистентной истории запуска заданий в Quartz AdminUi.
        /// </summary>
        public bool EnableAdminUiPersistentJobHistory { get; set; }

        /// <summary>
        /// TTL записи в истории запуска заданий в Quartz AdminUI.
        /// </summary>
        public int? PersistentRecentHistoryEntryTtl { get; set; }

        /// <summary>
        /// Настройки восстановления воркеров, находящихся в состоянии Blocked.
        /// </summary>
        public BlockedJobsRecoveryOptions BlockedJobsRecovery { get; set; }

        /// <summary>
        /// AdminUI URL.
        /// </summary>
        public string AdminUiUrl { get; set; } = "/dashboard";

        /// <summary>
        /// AdminUI URL Prefix (если приложение развернуто не в root сайта).
        /// </summary>
        public string AdminUiUrlPrefix { get; set; }

        public void RegisterJobsFromAssemblyContaining<T>()
        {
            JobRegister.RegisterJobs(typeof(T).Assembly);
        }
    }
}
