// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Concurrent;

using Gems.Data.UnitOfWork;

using Renci.SshNet;

namespace Gems.Data.MySql;

public static class SshAgent
{
    private static readonly ConcurrentDictionary<SshClientOptions, SshClient> SshClients =
        new ConcurrentDictionary<SshClientOptions, SshClient>();

    public static void OpenSsh(SshClientOptions sshClientOptions)
    {
        if (sshClientOptions == null)
        {
            return;
        }

        if (!SshClients.TryGetValue(sshClientOptions, out var sshClient))
        {
            SshClients.TryAdd(sshClientOptions, CreateSshClient(sshClientOptions));
            return;
        }

        if (sshClient is { IsConnected: true })
        {
            return;
        }

        sshClient.Connect();
    }

    private static SshClient CreateSshClient(SshClientOptions sshClientOptions)
    {
        var sshClient = new SshClient(
            new ConnectionInfo(
                sshClientOptions.SshHostName,
                sshClientOptions.SshPort,
                sshClientOptions.SshUserName,
                new AuthenticationMethod[] { new PasswordAuthenticationMethod(sshClientOptions.SshUserName, sshClientOptions.SshPassword) }));

        sshClient.Connect();

        var forwardedPort = new ForwardedPortLocal(
            sshClientOptions.ForwardedHostAddress,
            sshClientOptions.ForwardedDatabasePort,
            sshClientOptions.ForwardedDatabaseServer,
            sshClientOptions.Port);

        sshClient.AddForwardedPort(forwardedPort);
        forwardedPort.Start();

        return sshClient;
    }
}
