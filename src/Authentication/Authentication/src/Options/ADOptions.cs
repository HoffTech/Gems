// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Authentication.Options
{
    /// <summary>
    /// Информация о подключении к AD.
    /// </summary>
    public class ADOptions
    {
        /// <summary>
        /// Name in appsettings.json.
        /// </summary>
        public const string AD = "AD";

        /// <summary>
        /// Authority.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Authority.
        /// </summary>
        public string ValidIssuer { get; set; }

        /// <summary>
        /// ClientId.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// CertFileName.
        /// </summary>
        public string CertFileName { get; set; }
    }
}
