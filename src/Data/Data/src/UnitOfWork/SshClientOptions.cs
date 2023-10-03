// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Data.UnitOfWork;

public class SshClientOptions
{
    public string ForwardedHostAddress { get; set; }

    public string ForwardedDatabaseServer { get; set; }

    public uint ForwardedDatabasePort { get; set; }

    public string SshHostName { get; set; }

    public string SshUserName { get; set; }

    public string SshPassword { get; set; }

    public int SshPort { get; set; }

    public uint Port { get; set; }
}
