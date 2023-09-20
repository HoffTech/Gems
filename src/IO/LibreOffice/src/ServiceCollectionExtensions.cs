// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.IO.LibreOffice.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.IO.LibreOffice
{
    public static class ServiceCollectionExtensions
    {
        public static void AddLibreOffice(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LibreOfficeOptions>(configuration.GetSection(nameof(LibreOfficeOptions)));
            services.AddSingleton<ILibreOffice, LibreOffice>();
        }
    }
}
