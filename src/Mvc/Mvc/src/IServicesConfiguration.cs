// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Mvc
{
    public interface IServicesConfiguration
    {
        void Configure(IServiceCollection services, IConfiguration configuration);
    }
}
