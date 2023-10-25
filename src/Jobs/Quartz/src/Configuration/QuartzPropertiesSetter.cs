// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Quartz;

namespace Gems.Jobs.Quartz.Configuration;

public static class QuartzPropertiesSetter
{
    public static void SetProperties(IPropertySetter quartzConfigurator, QuartzProperties quartzProperties)
    {
        if (quartzProperties == null)
        {
            return;
        }

        if (quartzProperties.ThreadPool != null)
        {
            SetThreadPoolProperties(quartzConfigurator, quartzProperties.ThreadPool);
        }

        if (quartzProperties.Scheduler != null)
        {
            SetSchedulerProperties(quartzConfigurator, quartzProperties.Scheduler);
        }

        if (quartzProperties.Scheduler?.SchedulerExporter != null)
        {
            SetSchedulerExporterProperties(quartzConfigurator, quartzProperties.Scheduler.SchedulerExporter);
        }

        if (quartzProperties.JobStore != null)
        {
            SetJobStoreProperties(quartzConfigurator, quartzProperties.JobStore);
        }

        if (quartzProperties.DataSource == null)
        {
            return;
        }

        if (quartzProperties.DataSource.SqlServer != null)
        {
            SetSqlDataSourceProperties(quartzConfigurator, quartzProperties.DataSource.SqlServer, "SqlServer");
        }

        if (quartzProperties.DataSource.OracleOdp != null)
        {
            SetSqlDataSourceProperties(quartzConfigurator, quartzProperties.DataSource.OracleOdp, "OracleODP");
        }

        if (quartzProperties.DataSource.OracleOdpManaged != null)
        {
            SetSqlDataSourceProperties(quartzConfigurator, quartzProperties.DataSource.OracleOdpManaged, "OracleODPManaged");
        }

        if (quartzProperties.DataSource.MySql != null)
        {
            SetSqlDataSourceProperties(quartzConfigurator, quartzProperties.DataSource.MySql, "MySql");
        }

        if (quartzProperties.DataSource.Sqlite != null)
        {
            SetSqlDataSourceProperties(quartzConfigurator, quartzProperties.DataSource.Sqlite, "SQLite");
        }

        if (quartzProperties.DataSource.SqliteMicrosoft != null)
        {
            SetSqlDataSourceProperties(quartzConfigurator, quartzProperties.DataSource.SqliteMicrosoft, "SQLite-Microsoft");
        }

        if (quartzProperties.DataSource.Firebird != null)
        {
            SetSqlDataSourceProperties(quartzConfigurator, quartzProperties.DataSource.Firebird, "Firebird");
        }

        if (quartzProperties.DataSource.Npgsql != null)
        {
            SetSqlDataSourceProperties(quartzConfigurator, quartzProperties.DataSource.Npgsql, "Npgsql");
        }
    }

    private static void SetThreadPoolProperties(IPropertySetter quartzConfigurator, ThreadPoolOptions threadPoolOptions)
    {
        if (threadPoolOptions.Type != null)
        {
            quartzConfigurator.SetProperty("quartz.threadPool.type", threadPoolOptions.Type);
        }

        if (threadPoolOptions.MaxConcurrency != null)
        {
            quartzConfigurator.SetProperty("quartz.threadPool.maxConcurrency", threadPoolOptions.MaxConcurrency.ToString());
        }
    }

    private static void SetSqlDataSourceProperties(IPropertySetter quartzConfigurator, SqlDataSourceOptions sqlDataSourceOptions, string type)
    {
        if (sqlDataSourceOptions.Provider != null)
        {
            quartzConfigurator.SetProperty($"quartz.dataSource.{type}.provider", sqlDataSourceOptions.Provider);
        }

        if (sqlDataSourceOptions.ConnectionString != null)
        {
            quartzConfigurator.SetProperty($"quartz.dataSource.{type}.connectionString", sqlDataSourceOptions.ConnectionString);
        }

        if (sqlDataSourceOptions.ConnectionStringName != null)
        {
            quartzConfigurator.SetProperty($"quartz.dataSource.{type}.connectionStringName", sqlDataSourceOptions.ConnectionStringName);
        }

        if (sqlDataSourceOptions.ConnectionProviderType != null)
        {
            quartzConfigurator.SetProperty($"quartz.dataSource.{type}.connectionProvider.type", sqlDataSourceOptions.ConnectionProviderType);
        }
    }

    private static void SetJobStoreProperties(IPropertySetter quartzConfigurator, JobStoreOptions jobStoreOptions)
    {
        if (jobStoreOptions.MisfireThreshold != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.misfireThreshold", jobStoreOptions.DbRetryInterval.ToString());
        }

        if (jobStoreOptions.DbRetryInterval != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.dbRetryInterval", jobStoreOptions.DbRetryInterval.ToString());
        }

        if (jobStoreOptions.DriverDelegateType != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.driverDelegateType", jobStoreOptions.DriverDelegateType);
        }

        if (jobStoreOptions.DataSource != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.dataSource", jobStoreOptions.DataSource);
        }

        if (jobStoreOptions.TablePrefix != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.tablePrefix", jobStoreOptions.TablePrefix);
        }

        if (jobStoreOptions.UseProperties != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.useProperties", jobStoreOptions.UseProperties.ToString().ToLower());
        }

        if (jobStoreOptions.Clustered != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.clustered", jobStoreOptions.Clustered.ToString().ToLower());
        }

        if (jobStoreOptions.ClusterCheckinInterval != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.clusterCheckinInterval", jobStoreOptions.ClusterCheckinInterval.ToString());
        }

        if (jobStoreOptions.MaxMisfiresToHandleAtATime != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.maxMisfiresToHandleAtATime", jobStoreOptions.MaxMisfiresToHandleAtATime.ToString());
        }

        if (jobStoreOptions.SelectWithLockSql != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.selectWithLockSQL", jobStoreOptions.SelectWithLockSql);
        }

        if (jobStoreOptions.TxIsolationLevelSerializable != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.txIsolationLevelSerializable", jobStoreOptions.TxIsolationLevelSerializable.ToString().ToLower());
        }

        if (jobStoreOptions.AcquireTriggersWithinLock != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.acquireTriggersWithinLock", jobStoreOptions.AcquireTriggersWithinLock.ToString().ToLower());
        }

        if (jobStoreOptions.LockHandlerType != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.lockHandler.type", jobStoreOptions.LockHandlerType);
        }

        if (jobStoreOptions.DriverDelegateInitString != null)
        {
            quartzConfigurator.SetProperty("quartz.jobStore.driverDelegateInitString", jobStoreOptions.DriverDelegateInitString);
        }
    }

    private static void SetSchedulerProperties(IPropertySetter quartzConfigurator, SchedulerOptions schedulerOptions)
    {
        if (schedulerOptions.InstanceName != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.instanceName", schedulerOptions.InstanceName);
        }

        if (schedulerOptions.InstanceId != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.instanceId", schedulerOptions.InstanceId);
        }

        if (schedulerOptions.InstanceIdGeneratorType != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.instanceIdGenerator.type", schedulerOptions.InstanceIdGeneratorType);
        }

        if (schedulerOptions.ThreadName != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.threadName", schedulerOptions.ThreadName);
        }

        if (schedulerOptions.MakeSchedulerThreadDaemon != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.makeSchedulerThreadDaemon", schedulerOptions.MakeSchedulerThreadDaemon.ToString().ToLower());
        }

        if (schedulerOptions.IdleWaitTime != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.idleWaitTime", schedulerOptions.IdleWaitTime.ToString());
        }

        if (schedulerOptions.TypeLoadHelperType != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.typeLoadHelper.type", schedulerOptions.TypeLoadHelperType);
        }

        if (schedulerOptions.JobFactoryType != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.jobFactory.type", schedulerOptions.JobFactoryType);
        }

        if (schedulerOptions.WrapJobExecutionInUserTransaction != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.wrapJobExecutionInUserTransaction", schedulerOptions.WrapJobExecutionInUserTransaction.ToString().ToLower());
        }

        if (schedulerOptions.BatchTriggerAcquisitionMaxCount != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.batchTriggerAcquisitionMaxCount", schedulerOptions.BatchTriggerAcquisitionMaxCount.ToString());
        }

        if (schedulerOptions.BatchTriggerAcquisitionFireAheadTimeWindow != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.batchTriggerAcquisitionFireAheadTimeWindow", schedulerOptions.BatchTriggerAcquisitionFireAheadTimeWindow.ToString());
        }
    }

    private static void SetSchedulerExporterProperties(IPropertySetter quartzConfigurator, SchedulerExporterOptions schedulerExporterOptions)
    {
        if (schedulerExporterOptions.Type != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.exporter.type", schedulerExporterOptions.Type);
        }

        if (schedulerExporterOptions.Port != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.exporter.port", schedulerExporterOptions.Port.ToString());
        }

        if (schedulerExporterOptions.BindName != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.exporter.bindName", schedulerExporterOptions.BindName);
        }

        if (schedulerExporterOptions.ChannelType != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.exporter.channelType", schedulerExporterOptions.ChannelType);
        }

        if (schedulerExporterOptions.ChannelName != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.exporter.channelName", schedulerExporterOptions.ChannelName);
        }

        if (schedulerExporterOptions.TypeFilterLevel != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.exporter.typeFilterLevel", schedulerExporterOptions.TypeFilterLevel);
        }

        if (schedulerExporterOptions.RejectRemoteRequests != null)
        {
            quartzConfigurator.SetProperty("quartz.scheduler.exporter.rejectRemoteRequests", schedulerExporterOptions.RejectRemoteRequests.ToString());
        }
    }
}
