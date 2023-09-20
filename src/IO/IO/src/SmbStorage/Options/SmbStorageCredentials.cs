// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.IO.SmbStorage.Options
{
    public class SmbStorageCredentials
    {
        /// <summary>
        /// Наименование домена.
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// Логин.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Пароль.
        /// </summary>
        public string Password { get; set; }
    }
}
