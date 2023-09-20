// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace Gems.Extensions.DependencyInjection
{
    /// <summary>
    /// Расширение для <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Находит дескриптор указанного типа и при его наличии удаляет его.
        /// </summary>
        /// <param name="services">коллекция сервисов.</param>
        /// <param name="type">тип для поиска.</param>
        public static void RemoveServiceDescriptor(this IServiceCollection services, Type type)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == type);

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        }
    }
}
