// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Renci.SshNet;

namespace Gems.Data.MySql;

public class SshClientFactory
{
    private readonly UnitOfWork.SshClientOptions sshClientOptions;

    public SshClientFactory(UnitOfWork.SshClientOptions sshClientOptions)
    {
        this.sshClientOptions = sshClientOptions ?? throw new ArgumentNullException();
    }

    public Renci.SshNet.SshClient GetSshClient()
    {
        var sshClient = new Renci.SshNet.SshClient(
            new ConnectionInfo(
                this.sshClientOptions.SshHostName,
                this.sshClientOptions.SshPort,
                this.sshClientOptions.SshUserName,
                new AuthenticationMethod[] { new PasswordAuthenticationMethod(this.sshClientOptions.SshUserName, this.sshClientOptions.SshPassword) }));

        sshClient.Connect();

        var forwardedPort = new ForwardedPortLocal(
            this.sshClientOptions.ForwardedHostAddress,
            this.sshClientOptions.ForwardedDatabasePort,
            this.sshClientOptions.ForwardedDatabaseServer,
            this.sshClientOptions.Port);

        sshClient.AddForwardedPort(forwardedPort);
        forwardedPort.Start();

        return sshClient;
    }
}
