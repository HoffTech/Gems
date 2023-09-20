// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gems.IO.SmbStorage.Options;

using Microsoft.Extensions.Options;

using SMBLibrary;

using FileAttributes = SMBLibrary.FileAttributes;

namespace Gems.IO.SmbStorage
{
    public class SmbStorageService : ISmbStorageService
    {
        private readonly IOptions<SmbStorageOptions> options;

        public SmbStorageService() { }

        public SmbStorageService(IOptions<SmbStorageOptions> options)
        {
            this.options = options;
        }

        /// <summary>
        /// Наименование папки общего доступа.
        /// </summary>
        protected virtual string ShareName => this.options?.Value?.ShareName ?? string.Empty;

        /// <summary>
        /// Наименование сервера.
        /// </summary>
        /// <remarks>
        /// IP адрес или Хост.
        /// </remarks>
        protected virtual string ServerName => this.options?.Value?.ServerName ?? string.Empty;

        /// <summary>
        /// Наименование домена.
        /// </summary>
        /// <remarks>
        /// IP адрес или Хост.
        /// </remarks>
        protected virtual string DomainName => this.options?.Value?.Credentials?.DomainName ?? string.Empty;

        /// <summary>
        /// Логин.
        /// </summary>
        protected virtual string UserName => this.options?.Value?.Credentials?.UserName ?? string.Empty;

        /// <summary>
        /// Пароль.
        /// </summary>
        protected virtual string Password => this.options?.Value?.Credentials?.Password ?? string.Empty;

        /// <inheritdoc></inheritdoc>
        public virtual bool CheckFileExists(string filePath, string basePath = null)
        {
            using var connection = new Connection(this.options.Value, basePath);
            var status = connection.FileStore.CreateFile(
                out var fileHandle,
                out _,
                filePath,
                AccessMask.GENERIC_READ,
                0,
                ShareAccess.Read,
                CreateDisposition.FILE_OPEN,
                CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT,
                null);

            switch (status)
            {
                case NTStatus.STATUS_SUCCESS:
                    connection.FileStore.CloseFile(fileHandle);
                    return true;
                case NTStatus.STATUS_NOT_FOUND:
                case NTStatus.STATUS_NO_SUCH_FILE:
                case NTStatus.STATUS_OBJECT_NAME_NOT_FOUND:
                case NTStatus.STATUS_OBJECT_PATH_NOT_FOUND:
                    return false;
                default:
                    throw new SmbException(
                        $"При проверке наличия файла по пути \"{filePath}\" произошла ошибка. Статус: {status}");
            }
        }

        /// <inheritdoc></inheritdoc>
        public virtual IEnumerable<string> GetFileNamesInFolder(string directoryPath, string fileNamePattern = "*", string basePath = null)
        {
            using var connection = new Connection(this.options.Value, basePath);

            if (!this.CheckDirectoryExists(directoryPath, basePath))
            {
                return Enumerable.Empty<string>();
            }

            var status = connection.FileStore.CreateFile(
                out var directoryHandle,
                out var fileStatus,
                directoryPath,
                AccessMask.GENERIC_READ,
                FileAttributes.Directory,
                ShareAccess.Read | ShareAccess.Write,
                CreateDisposition.FILE_OPEN,
                CreateOptions.FILE_DIRECTORY_FILE,
                null);

            if (status != NTStatus.STATUS_SUCCESS)
            {
                throw new SmbException($"При чтении данных о файлах по пути \"{directoryPath}\" произошла ошибка. Статус: {status}");
            }

            connection.FileStore.QueryDirectory(out var fileList, directoryHandle, fileNamePattern, FileInformationClass.FileDirectoryInformation);
            connection.FileStore.CloseFile(directoryHandle);
            return fileList.Select(fi => (fi as FileDirectoryInformation)?.FileName);
        }

        /// <inheritdoc></inheritdoc>
        public virtual void DeleteFileIfExists(string filePath, string basePath = null)
        {
            if (!this.CheckFileExists(filePath, basePath))
            {
                return;
            }

            using var connection = new Connection(this.options.Value, basePath);
            var status = connection.FileStore.CreateFile(
                out var file,
                out _,
                filePath,
                AccessMask.GENERIC_WRITE | AccessMask.DELETE | AccessMask.SYNCHRONIZE,
                FileAttributes.Normal,
                ShareAccess.None,
                CreateDisposition.FILE_OPEN,
                CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT,
                null);

            if (status != NTStatus.STATUS_SUCCESS)
            {
                throw new SmbException($"При удалении файла по пути \"{filePath}\" произошла ошибка. Статус: {status}");
            }

            connection.FileStore.SetFileInformation(file, new FileDispositionInformation { DeletePending = true });
            connection.FileStore.CloseFile(file);
        }

        /// <inheritdoc></inheritdoc>
        public virtual Task<Stream> ReadFileAsStreamAsync(
            string filePath,
            string basePath,
            CancellationToken cancellationToken)
        {
            return this.ReadFileInternalAsync(filePath, basePath, cancellationToken);
        }

        /// <inheritdoc></inheritdoc>
        public virtual Task<Stream> ReadFileAsStreamAsync(
            string filePath,
            CancellationToken cancellationToken)
        {
            return this.ReadFileInternalAsync(filePath, null, cancellationToken);
        }

        /// <inheritdoc></inheritdoc>
        public virtual async Task<byte[]> ReadFileAsByteArrayAsync(
            string filePath,
            string basePath,
            CancellationToken cancellationToken)
        {
            await using var stream = await this
                .ReadFileInternalAsync(filePath, basePath, cancellationToken)
                .ConfigureAwait(false);
            return ((MemoryStream)stream)?.ToArray();
        }

        /// <inheritdoc></inheritdoc>
        public virtual async Task<byte[]> ReadFileAsByteArrayAsync(
            string filePath,
            CancellationToken cancellationToken)
        {
            await using var stream = await this.ReadFileInternalAsync(filePath, null, cancellationToken).ConfigureAwait(false);
            return ((MemoryStream)stream)?.ToArray();
        }

        /// <inheritdoc></inheritdoc>
        public virtual Task WriteFileAsync(
            string filePath,
            byte[] data,
            CancellationToken cancellationToken)
        {
            return this.WriteFileInternalAsync(filePath, null, data, cancellationToken);
        }

        /// <inheritdoc></inheritdoc>
        public virtual Task WriteFileAsync(
            string filePath,
            string basePath,
            byte[] data,
            CancellationToken cancellationToken)
        {
            return this.WriteFileInternalAsync(filePath, basePath, data, cancellationToken);
        }

        /// <inheritdoc></inheritdoc>
        public virtual bool CheckDirectoryExists(string directoryPath)
        {
            return this.CheckDirectoryExistsInternal(directoryPath);
        }

        /// <inheritdoc></inheritdoc>
        public virtual bool CheckDirectoryExists(string directoryPath, string basePath)
        {
            return this.CheckDirectoryExistsInternal(directoryPath, basePath);
        }

        /// <inheritdoc></inheritdoc>
        public virtual void CreateDirectoryIfNotExists(string directoryPath)
        {
            this.CreateDirectoryIfNotExistsInternal(directoryPath);
        }

        /// <inheritdoc></inheritdoc>
        public virtual void CreateDirectoryIfNotExists(string directoryPath, string basePath)
        {
            this.CreateDirectoryIfNotExistsInternal(directoryPath, basePath);
        }

        private static string ExtractLatestDirectoryName(string path)
        {
            return path.Split('\\').Last();
        }

        private static string ExtractComplexDirectoryName(string path)
        {
            var index = path.LastIndexOf('\\');
            return index < 0 ? string.Empty : path[..index];
        }

        private bool CheckDirectoryExistsInternal(string directoryPath, string basePath = null)
        {
            using var connection = new Connection(this.options.Value, basePath);
            var status = connection.FileStore.CreateFile(
                out var fileHandle,
                out _,
                directoryPath,
                AccessMask.GENERIC_READ,
                0,
                ShareAccess.Read,
                CreateDisposition.FILE_OPEN,
                CreateOptions.FILE_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT,
                null);

            switch (status)
            {
                case NTStatus.STATUS_SUCCESS:
                    connection.FileStore.CloseFile(fileHandle);
                    return true;
                case NTStatus.STATUS_NOT_FOUND:
                case NTStatus.STATUS_NO_SUCH_FILE:
                case NTStatus.STATUS_OBJECT_NAME_NOT_FOUND:
                case NTStatus.STATUS_OBJECT_PATH_NOT_FOUND:
                    return false;
                default:
                    throw new SmbException(
                        $"При проверке наличия директории по пути \"{directoryPath}\" произошла ошибка. Статус: {status}");
            }
        }

        private void CreateDirectoryIfNotExistsInternal(string directoryPath, string basePath = null)
        {
            var directories = new Stack<string>();
            while (!string.IsNullOrEmpty(directoryPath))
            {
                directories.Push(ExtractLatestDirectoryName(directoryPath));
                directoryPath = ExtractComplexDirectoryName(directoryPath);
            }

            var path = string.Empty;
            while (directories.Count > 0)
            {
                var tmpDirectoryName = directories.Pop();
                path = Path.Combine(path, tmpDirectoryName).Replace('/', '\\');
                if (!this.CheckDirectoryExists(path, basePath))
                {
                    this.CreateDirectory(path, basePath);
                }
            }
        }

        private async Task WriteFileInternalAsync(
            string filePath,
            string basePath,
            byte[] data,
            CancellationToken cancellationToken)
        {
            if (filePath.Split('\\').Length > 0)
            {
                this.CreateDirectoryIfNotExists(filePath[..filePath.LastIndexOf("\\", StringComparison.Ordinal)]);
            }

            using var connection = new Connection(this.options.Value, basePath);
            var status = connection.FileStore.CreateFile(
                out var fileHandle,
                out _,
                filePath,
                AccessMask.GENERIC_WRITE | AccessMask.SYNCHRONIZE,
                FileAttributes.Normal,
                ShareAccess.None,
                CreateDisposition.FILE_OVERWRITE_IF,
                CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_NONALERT,
                null);

            if (status != NTStatus.STATUS_SUCCESS)
            {
                throw new SmbException($"При записи файла \"{filePath}\" произошла ошибка Статус: {status}");
            }

            var buffer = new byte[connection.Client.MaxWriteSize < data.Length ? (int)connection.Client.MaxWriteSize : data.Length];
            var offset = 0;
            await using var stream = new MemoryStream(data);
            while (true)
            {
                var size = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                if (size <= 0)
                {
                    break;
                }

                status = connection.FileStore.WriteFile(
                    out _,
                    fileHandle,
                    offset,
                    size == buffer.Length ? buffer : buffer.Take(size).ToArray());

                if (status != NTStatus.STATUS_SUCCESS)
                {
                    throw new SmbException($"При записи файла \"{filePath}\" произошла ошибка Статус: {status}");
                }

                offset += size;
            }

            connection.FileStore.CloseFile(fileHandle);
        }

        private async Task<Stream> ReadFileInternalAsync(
            string filePath,
            string basePath,
            CancellationToken cancellationToken)
        {
            if (!this.CheckFileExists(filePath, basePath))
            {
                return null;
            }

            using var connection = new Connection(this.options.Value, basePath);
            var status = connection.FileStore.CreateFile(
                out var file,
                out _,
                filePath,
                AccessMask.GENERIC_READ | AccessMask.SYNCHRONIZE,
                FileAttributes.Normal,
                ShareAccess.Read | ShareAccess.Write,
                CreateDisposition.FILE_OPEN,
                CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT,
                null);

            if (status != NTStatus.STATUS_SUCCESS)
            {
                throw new SmbException($"При чтении файла по пути \"{filePath}\" произошла ошибка. Статус: {status}");
            }

            var stream = new MemoryStream();
            long offset = 0;
            while (true)
            {
                status = connection.FileStore.ReadFile(out var data, file, offset, (int)connection.Client.MaxReadSize);
                if (status != NTStatus.STATUS_SUCCESS && status != NTStatus.STATUS_END_OF_FILE)
                {
                    throw new SmbException($"При попытке чтения файла по пути \"{filePath}\" произошла ошибка. Статус: {status}");
                }

                if (status == NTStatus.STATUS_END_OF_FILE || data.Length == 0)
                {
                    break;
                }

                offset += data.Length;
                await stream.WriteAsync(data, cancellationToken).ConfigureAwait(false);
            }

            connection.FileStore.CloseFile(file);
            return stream;
        }

        private void CreateDirectory(string directoryPath, string basePath = null)
        {
            using var connection = new Connection(this.options.Value, basePath);
            var status = connection.FileStore.CreateFile(
                out var fileHandle,
                out _,
                directoryPath,
                AccessMask.GENERIC_WRITE | AccessMask.SYNCHRONIZE,
                0,
                ShareAccess.None,
                CreateDisposition.FILE_CREATE,
                CreateOptions.FILE_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT,
                null);

            if (status == NTStatus.STATUS_SUCCESS)
            {
                connection.FileStore.CloseFile(fileHandle);
            }
            else
            {
                throw new SmbException($"При создании директории \"{directoryPath}\" произошла ошибка. Статус: {status}");
            }
        }
    }
}
