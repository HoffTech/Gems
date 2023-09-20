// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.IO.SmbStorage.Options
{
    public class SmbStorageOptions
    {
        /// <summary>
        /// Наименование папки общего доступа.
        /// </summary>
        public string ShareName { get; set; }

        /// <summary>
        /// Наименование сервера.
        /// </summary>
        /// <remarks>
        /// IP адрес или Хост.
        /// </remarks>
        public string ServerName { get; set; }

        /// <summary>
        /// Данные для авторизации в храналище.
        /// </summary>
        public SmbStorageCredentials Credentials { get; set; }
    }
}
