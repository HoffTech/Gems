// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Jobs.Quartz.Configuration;

public class SchedulerOptions
{
    public string InstanceName { get; set; }

    public string InstanceId { get; set; }

    public string InstanceIdGeneratorType { get; set; }

    public string ThreadName { get; set; }

    public bool? MakeSchedulerThreadDaemon { get; set; }

    public long? IdleWaitTime { get; set; }

    public string TypeLoadHelperType { get; set; }

    public string JobFactoryType { get; set; }

    public bool? WrapJobExecutionInUserTransaction { get; set; }

    public int? BatchTriggerAcquisitionMaxCount { get; set; }

    public long? BatchTriggerAcquisitionFireAheadTimeWindow { get; set; }

    public SchedulerExporterOptions SchedulerExporter { get; set; }
}
