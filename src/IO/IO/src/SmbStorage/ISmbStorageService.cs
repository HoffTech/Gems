// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gems.IO.SmbStorage
{
    /// <summary>
    /// Сервис для работы с файловым хранилищем по протоколу SMB.
    /// </summary>
    public interface ISmbStorageService
    {
        /// <summary>
        /// Метод проверки наличия файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="basePath">Путь формата \\ServerName\ShareName.</param>
        /// <returns>true - если файл найден. false - если файл не найден.</returns>
        bool CheckFileExists(string filePath, string basePath = null);

        /// <summary>
        /// Метод для получения названий файлов в указанной директории.
        /// </summary>
        /// <param name="directoryPath">Путь к папке.</param>
        /// <param name="fileNamePattern">Шаблон названия файла или само название.</param>
        /// <param name="basePath">Путь формата \\ServerName\ShareName.</param>
        /// <returns>Названия файлов в указанной директории.</returns>
        /// <exception cref="SmbException">Smb ошибка при чтении данных о файлах.</exception>
        IEnumerable<string> GetFileNamesInFolder(string directoryPath, string fileNamePattern = "*", string basePath = null);

        /// <summary>
        /// Метод удаления файла если существует.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="basePath">Путь формата \\ServerName\ShareName.</param>
        /// <exception cref="SmbException">Smb ошибка при удалении.</exception>
        void DeleteFileIfExists(string filePath, string basePath = null);

        /// <summary>
        /// Метод чтения файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="basePath">Путь формата \\ServerName\ShareName.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Файл в виде потока.</returns>
        Task<Stream> ReadFileAsStreamAsync(
            string filePath,
            string basePath,
            CancellationToken cancellationToken);

        /// <summary>
        /// Метод чтения файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Файл в виде потока.</returns>
        Task<Stream> ReadFileAsStreamAsync(string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Метод чтения файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Файл в виде массива байтов.</returns>
        Task<byte[]> ReadFileAsByteArrayAsync(string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Метод чтения файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="basePath">Путь формата \\ServerName\ShareName.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Файл в виде массива байтов.</returns>
        public Task<byte[]> ReadFileAsByteArrayAsync(
            string filePath,
            string basePath,
            CancellationToken cancellationToken);

        /// <summary>
        /// Метод записи файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="data">Данные.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task WriteFileAsync(string filePath, byte[] data, CancellationToken cancellationToken);

        /// <summary>
        /// Метод записи файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="basePath">Путь формата \\ServerName\ShareName.</param>
        /// <param name="data">Данные.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        public Task WriteFileAsync(
            string filePath,
            string basePath,
            byte[] data,
            CancellationToken cancellationToken);

        /// <summary>
        /// Метод проверки наличия директории.
        /// </summary>
        /// <param name="directoryPath">Путь к директории.</param>
        /// <returns>Признак наличия директории.</returns>
        bool CheckDirectoryExists(string directoryPath);

        /// <summary>
        /// Метод проверки наличия директории.
        /// </summary>
        /// <param name="directoryPath">Путь к директории.</param>
        /// <param name="basePath">Путь формата \\ServerName\ShareName.</param>
        /// <returns>Признак наличия директории.</returns>
        bool CheckDirectoryExists(string directoryPath, string basePath);

        /// <summary>
        /// Метод создания директории если не существует.
        /// </summary>
        /// <remarks>
        /// Создает вложенный директории если directoryPath составной.
        /// </remarks>
        /// <param name="directoryPath">Путь к директории.</param>
        void CreateDirectoryIfNotExists(string directoryPath);

        /// <summary>
        /// Метод создания директории если не существует.
        /// </summary>
        /// <remarks>
        /// Создает вложенный директории если directoryPath составной.
        /// </remarks>
        /// <param name="directoryPath">Путь к директории.</param>
        /// <param name="basePath">Путь формата \\ServerName\ShareName.</param>
        void CreateDirectoryIfNotExists(string directoryPath, string basePath);
    }
}
