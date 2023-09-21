// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Globalization;
using System.Linq;

using FluentValidation;
using FluentValidation.AspNetCore;

using Gems.Mvc.Filters;
using Gems.Mvc.Filters.Errors;
using Gems.Mvc.GenericControllers;
using Gems.Mvc.MultipleModelBinding;
using Gems.Mvc.Validation;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gems.Mvc
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for controllers to the specified <see cref="IServiceCollection"/>. This method will not
        /// register services used for views or pages.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configureOptions">An <see cref="Action{MvcOptions}"/> to configure the provided <see cref="MvcOptions"/>.</param>
        /// <param name="configuration">configuration.</param>
        /// <returns>An <see cref="IMvcBuilder"/> that can be used to further configure the MVC services.</returns>
        public static IMvcBuilder AddControllersWithMediatR(
            this IServiceCollection services,
            Action<MvcOptions> configureOptions = null,
            IConfiguration configuration = null)
        {
            services.ConfigureOptions<MultipleModelBinderSetup>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.Configure<ExceptionHandlingOptions>(configuration?.GetSection(ExceptionHandlingOptions.SectionName));

            services.AddSingleton<IConverter<Exception, BusinessErrorViewModel>, ExceptionToBusinessErrorViewModelConverter>();
            return services.AddControllers(options =>
                {
                    options.Filters.Add(typeof(ModelStateValidationFilter));
                    options.Filters.Add(typeof(HandleErrorFilter));
                    options.Conventions.Add(new GenericControllerRouteConvention());
                    configureOptions?.Invoke(options);
                })
                .ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider()));
        }

        /// <summary>
        /// Adds services for controllers to the specified <see cref="IServiceCollection"/>. This method will not
        /// register services used for views or pages.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configureOptions">An <see cref="Action{MvcOptions}"/> to configure the provided <see cref="MvcOptions"/>.</param>
        /// <param name="configuration">configuration.</param>
        public static void AddControllersWithViewsAndMediatR(
            this IServiceCollection services,
            Action<MvcOptions> configureOptions = null,
            IConfiguration configuration = null)
        {
            services.ConfigureOptions<MultipleModelBinderSetup>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.Configure<ExceptionHandlingOptions>(configuration?.GetSection(ExceptionHandlingOptions.SectionName));

            services.AddSingleton<IConverter<Exception, BusinessErrorViewModel>, ExceptionToBusinessErrorViewModelConverter>();
            services.AddControllersWithViews(options =>
                {
                    options.Filters.Add(typeof(ModelStateValidationFilter));
                    options.Filters.Add(typeof(HandleErrorFilter));
                    options.Conventions.Add(new GenericControllerRouteConvention());
                    configureOptions?.Invoke(options);
                })
                .ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider()));
        }

        public static void RegisterControllersFromAssemblyContaining<T>(this MvcOptions mvcOptions, string endpointStartForHiding = null)
        {
            if (mvcOptions is null)
            {
                throw new ArgumentNullException(nameof(mvcOptions));
            }

            ControllerRegister.RegisterControllers(endpointStartForHiding, typeof(T).Assembly);
        }

        public static void RegisterServicesFromAssemblyContaining<T>(this IServiceCollection services, IConfiguration configuration)
        {
            var interfaceType = typeof(IServicesConfiguration);
            var servicesConfigurations = typeof(T).Assembly
                .GetTypes()
                .Where(x => x.GetInterfaces().Contains(interfaceType))
                .Select(x => Activator.CreateInstance(x))
                .Cast<IServicesConfiguration>();
            foreach (var servicesConfiguration in servicesConfigurations)
            {
                servicesConfiguration.Configure(services, configuration);
            }
        }

        public static void AddPipeline(this IServiceCollection services, Type behaviorType)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), behaviorType);
        }

        public static void AddValidation(this IServiceCollection services, Action<ValidationOptions> configureOptions)
        {
            var options = new ValidationOptions(services);
            configureOptions?.Invoke(options);

            ValidatorOptions.Global.LanguageManager.Culture = options.CultureInfo ?? new CultureInfo("ru");

            if (options.DisplayNameResolver == DisplayNameResolver.CamelCase)
            {
                ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) => member == null
                    ? null
                    : member.Name[0].ToString().ToLower() + member.Name[1..];
            }

            services.AddFluentValidationClientsideAdapters();
        }
    }
}
