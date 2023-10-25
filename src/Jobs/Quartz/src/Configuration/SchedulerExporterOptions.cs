// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Jobs.Quartz.Configuration;

public class SchedulerExporterOptions
{
    public string Type { get; set; }

    public int? Port { get; set; }

    public string BindName { get; set; }

    public string ChannelType { get; set; }

    public string ChannelName { get; set; }

    public string TypeFilterLevel { get; set; }

    public bool? RejectRemoteRequests { get; set; }
}
