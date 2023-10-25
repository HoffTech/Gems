// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Jobs.Quartz.Configuration;

public class QuartzProperties
{
    public ThreadPoolOptions ThreadPool { get; set; }

    public SchedulerOptions Scheduler { get; set; }

    public JobStoreOptions JobStore { get; set; }

    public DataSourceOptions DataSource { get; set; }
}
