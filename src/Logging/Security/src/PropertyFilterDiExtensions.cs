// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Microsoft.Extensions.DependencyInjection;

namespace Gems.Logging.Security
{
    public static class PropertyFilterDiExtensions
    {
        public static IServiceCollection AddLoggingFilter(
            this IServiceCollection services,
            Action<IPropertyFilterBuilder> build = null)
        {
            var secureKeyProvider = new SecureKeyProvider();
            services.AddSingleton<ISecureKeyProvider>(secureKeyProvider);
            var builder = new PropertyFilterBuilder(services, secureKeyProvider);
            builder.Register(new JsonPropertyVisitor());
            builder.Register(new XmlPropertyVisitor());
            builder.Register(new ObjectToJsonPropertyVisitor());
            var objectPropertyFilterSource = new ObjectPropertyFilterSource();
            builder.Register(objectPropertyFilterSource);
            services.AddSingleton<IObjectPropertyFilterSource>(objectPropertyFilterSource);
            build?.Invoke(builder);
            return services;
        }
    }
}
