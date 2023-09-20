// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.RegularExpressions;

using Gems.IO.SmbStorage.Options;

using SMBLibrary;
using SMBLibrary.Client;

namespace Gems.IO.SmbStorage
{
    public class Connection : IDisposable
    {
        private readonly SmbStorageOptions options;

        private bool isDisposed;

        private bool isClientConnected;
        private bool isLoggedIn;
        private bool isStorageConnected;

        public Connection(SmbStorageOptions options, string basePath = null)
        {
            this.options = options;
            this.Client = new SMB2Client();
            this.OpenConnection(basePath);
        }

        public ISMBFileStore FileStore { get; private set; }

        public SMB2Client Client { get; }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            if (this.isStorageConnected)
            {
                this.FileStore.Disconnect();
                this.isStorageConnected = false;
            }

            if (this.isLoggedIn)
            {
                this.Client.Logoff();
                this.isLoggedIn = false;
            }

            if (this.isClientConnected)
            {
                this.Client.Disconnect();
                this.isClientConnected = false;
            }

            this.isDisposed = true;
        }

        private void OpenConnection(string basePath = null)
        {
            var connectionData = this.GetBasePathConnectionData(basePath);

            if (!this.isClientConnected)
            {
                try
                {
                    this.isClientConnected = this.Client.Connect(connectionData.ServerName, SMBTransportType.DirectTCPTransport);
                    if (!this.isClientConnected)
                    {
                        this.isClientConnected = this.Client.Connect(connectionData.ServerName, SMBTransportType.NetBiosOverTCP);
                    }
                }
                catch (NotSupportedException)
                {
                    this.isClientConnected = this.Client.Connect(connectionData.ServerName, SMBTransportType.NetBiosOverTCP);
                }

                if (!this.isClientConnected)
                {
                    throw new SmbException($"Не удалось подключится к серверу: \"{connectionData.ServerName}\"");
                }
            }

            if (!this.isLoggedIn)
            {
                var loginStatus = this.Client.Login(
                    this.options.Credentials.DomainName,
                    this.options.Credentials.UserName,
                    this.options.Credentials.Password);
                if (loginStatus != NTStatus.STATUS_SUCCESS)
                {
                    throw new SmbException($"Пользователь {this.options.Credentials.UserName} " +
                                           $"не прошел авторизацию \"{this.options.Credentials.DomainName}\"." +
                                           $"Статус {loginStatus}");
                }

                this.isLoggedIn = true;
            }

            if (!this.isStorageConnected)
            {
                this.FileStore = this.Client.TreeConnect(connectionData.ShareName, out var status);
                if (status != NTStatus.STATUS_SUCCESS)
                {
                    throw new SmbException($"Не удалось подключится к папке \"\\{connectionData.ServerName}\\{connectionData.ShareName}\"" +
                                           $"Статус: {status}");
                }

                this.isStorageConnected = true;
            }
        }

        private BasePathConnection GetBasePathConnectionData(string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                return new BasePathConnection
                {
                    ServerName = this.options.ServerName,
                    ShareName = this.options.ShareName,
                };
            }

            var isMatch = Regex.IsMatch(basePath, @"(\\\\)\S*\\\S*");
            if (!isMatch)
            {
                throw new SmbException(
                    $"Путь к сетевой папке \"{basePath}\" не прошел проверку на соответствие паттерну \\\\ServerName\\ShareFolderName");
            }

            var split = basePath.TrimStart('\\').Split('\\');
            var basePathData = new BasePathConnection
            {
                ServerName = split[0],
                ShareName = split[1],
            };

            if (string.IsNullOrEmpty(basePathData.ServerName) || string.IsNullOrEmpty(basePathData.ShareName))
            {
                throw new SmbException(
                    "Один из параметров: " +
                    $"{nameof(basePathData.ServerName)}," +
                    $"{nameof(basePathData.ShareName)} не был заполнен после обработки." +
                    $"Переданный путь: {basePath}");
            }

            return basePathData;
        }
    }
}
