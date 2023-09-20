// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Globalization;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Gems.Mvc.Validation
{
    public class ValidationOptions
    {
        private readonly IServiceCollection services;

        public ValidationOptions(IServiceCollection services)
        {
            this.services = services;
        }

        public CultureInfo CultureInfo { get; set; }

        public DisplayNameResolver DisplayNameResolver { get; set; }

        public void RegisterValidatorsFromAssemblyContaining<T>()
        {
            this.services.AddValidatorsFromAssemblyContaining<T>();
        }
    }
}
