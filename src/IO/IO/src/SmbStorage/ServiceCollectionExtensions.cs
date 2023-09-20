// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.IO.SmbStorage.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.IO.SmbStorage
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление поддержки SMB сервиса.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        /// <param name="configuration">IConfiguration.</param>
        public static void AddSmbService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmbStorageOptions>(configuration.GetSection(nameof(SmbStorageOptions)));
            services.AddSingleton<ISmbStorageService, SmbStorageService>();
        }
    }
}
