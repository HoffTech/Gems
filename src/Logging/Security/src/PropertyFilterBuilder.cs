// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Gems.Logging.Security
{
    public sealed class PropertyFilterBuilder : IPropertyFilterBuilder
    {
        private readonly IServiceCollection services;
        private readonly SecureKeyProvider secureKeyProvider;

        internal PropertyFilterBuilder(IServiceCollection services, SecureKeyProvider secureKeyProvider)
        {
            this.services = services;
            this.secureKeyProvider = secureKeyProvider;
        }

        public IPropertyFilterBuilder Register<TElement>(IPropertyVisitor<TElement> propertyVisitor)
            where TElement : class
        {
            this.services.AddSingleton(propertyVisitor);
            this.services.AddSingleton<IPropertyFilter<TElement>>(sp =>
            {
                var visitor = sp.GetRequiredService<IPropertyVisitor<TElement>>();
                var secureKeyProvider = sp.GetRequiredService<ISecureKeyProvider>();
                return new PropertyFilter<TElement>(secureKeyProvider, visitor);
            });
            return this;
        }

        public IPropertyFilterBuilder Register(ISecureKeySource secureKeySource)
        {
            this.secureKeyProvider.Add(secureKeySource);
            return this;
        }
    }
}
