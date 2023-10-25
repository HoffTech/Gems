// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Jobs.Quartz.Configuration;

public class JobStoreOptions
{
    public int? MisfireThreshold { get; set; }

    public long? DbRetryInterval { get; set; }

    public string DriverDelegateType { get; set; }

    public string DataSource { get; set; }

    public string TablePrefix { get; set; }

    public bool? UseProperties { get; set; }

    public bool? Clustered { get; set; }

    public long? ClusterCheckinInterval { get; set; }

    public int? MaxMisfiresToHandleAtATime { get; set; }

    public string SelectWithLockSql { get; set; }

    public bool? TxIsolationLevelSerializable { get; set; }

    public bool? AcquireTriggersWithinLock { get; set; }

    public string LockHandlerType { get; set; }

    public string DriverDelegateInitString { get; set; }
}
