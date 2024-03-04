using Gems.TestInfrastructure.Environment;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Gems.TestInfrastructure.Integration
{
    public static class TestApplicationBuilderExtensions
    {
        public static ITestApplicationBuilder UseEnvironment(this ITestApplicationBuilder builder, ConfigurationEnvironment environment)
        {
            builder.UseEnvironment(environment.ToString());
            return builder;
        }

        public static ITestApplicationBuilder UseConnectionString(this ITestApplicationBuilder builder, string name, ITestEnvironment env)
        {
            builder.UseConnectionString(name, env.DatabaseConnectionString(name));
            return builder;
        }

        public static ITestApplicationBuilder UseConnectionString(this ITestApplicationBuilder builder, string name, string value)
        {
            builder.UseSetting($"ConnectionStrings:{name}", value);
            return builder;
        }

        public static ITestApplicationBuilder UseConnectionString(this ITestApplicationBuilder builder, string name, Uri uri)
        {
            builder.UseSetting($"ConnectionStrings:{name}", uri.ToString());
            return builder;
        }

        public static ITestApplicationBuilder UseSetting(this ITestApplicationBuilder builder, string name, Uri uri)
        {
            builder.UseSetting(name, uri.ToString());
            return builder;
        }

        public static ITestApplicationBuilder LogClearProviders(this ITestApplicationBuilder builder)
        {
            return builder.ConfigureLogging(b => b.ClearProviders());
        }

        public static ITestApplicationBuilder LogSetMinimumLevel(this ITestApplicationBuilder builder, LogLevel level)
        {
            return builder.ConfigureLogging(b => b.SetMinimumLevel(level));
        }

        public static ITestApplicationBuilder LogToConsole(this ITestApplicationBuilder builder, Action<ConsoleLoggerOptions> setup = default)
        {
            return builder.ConfigureLogging(b => b.AddConsole(cb => setup?.Invoke(cb)));
        }

        public static ITestApplicationBuilder LogToDebug(this ITestApplicationBuilder builder)
        {
            return builder.ConfigureLogging(b => b.AddDebug());
        }

        public static ITestApplicationBuilder RemoveServiceImplementation<TImplementation>(this ITestApplicationBuilder builder)
        {
            var implementationType = typeof(TImplementation);
            return builder.ConfigureServices(s => s
                .Where(x => x.ImplementationType == implementationType)
                .ToList()
                .ForEach(x => s.Remove(x)));
        }

        public static ITestApplicationBuilder RemoveService<TService>(this ITestApplicationBuilder builder)
        {
            return builder.ConfigureServices(s => s.RemoveAll<TService>());
        }

        public static ITestApplicationBuilder RemoveServiceImplementationByFullName(this ITestApplicationBuilder builder, string implementationTypeFullName)
        {
            return builder.ConfigureServices(s => s
                .Where(x => x.ImplementationType.FullName == implementationTypeFullName)
                .ToList()
                .ForEach(x => s.Remove(x)));
        }

        public static ITestApplicationBuilder RemoveServiceImplementationByName(this ITestApplicationBuilder builder, string implementationTypeName)
        {
            return builder.ConfigureServices(s => s
                .Where(x => x.ImplementationType.Name == implementationTypeName)
                .ToList()
                .ForEach(x => s.Remove(x)));
        }

        public static ITestApplicationBuilder RemoveServiceByFullName(this ITestApplicationBuilder builder, string serviceTypeFullName)
        {
            return builder.ConfigureServices(s => s
                .Where(x => x.ServiceType.FullName == serviceTypeFullName)
                .ToList()
                .ForEach(x => s.Remove(x)));
        }

        public static ITestApplicationBuilder RemoveServiceByName(this ITestApplicationBuilder builder, string serviceTypeName)
        {
            return builder.ConfigureServices(s => s
                .Where(x => x.ServiceType.Name == serviceTypeName)
                .ToList()
                .ForEach(x => s.Remove(x)));
        }

        public static ITestApplicationBuilder ReplaceServiceWithSingleton<TService, TImplementation>(this ITestApplicationBuilder builder)
            where TService : class
            where TImplementation : class, TService
        {
            return builder.ConfigureServices(s =>
            {
                s.RemoveAll<TService>();
                s.AddSingleton<TService, TImplementation>();
            });
        }

        public static ITestApplicationBuilder ReplaceServiceWithSingleton<TService, TImplementation>(
            this ITestApplicationBuilder builder,
            Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            return builder.ConfigureServices(s =>
            {
                s.RemoveAll<TService>();
                s.AddSingleton<TService, TImplementation>(factory);
            });
        }

        public static ITestApplicationBuilder ReplaceServiceWithScoped<TService, TImplementation>(this ITestApplicationBuilder builder)
            where TService : class
            where TImplementation : class, TService
        {
            return builder.ConfigureServices(s =>
            {
                s.RemoveAll<TService>();
                s.AddScoped<TService, TImplementation>();
            });
        }

        public static ITestApplicationBuilder ReplaceServiceWithScoped<TService, TImplementation>(
            this ITestApplicationBuilder builder,
            Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            return builder.ConfigureServices(s =>
            {
                s.RemoveAll<TService>();
                s.AddScoped<TService, TImplementation>(factory);
            });
        }

        public static ITestApplicationBuilder ReplaceServiceWithTransient<TService, TImplementation>(this ITestApplicationBuilder builder)
            where TService : class
            where TImplementation : class, TService
        {
            return builder.ConfigureServices(s =>
            {
                s.RemoveAll<TService>();
                s.AddTransient<TService, TImplementation>();
            });
        }

        public static ITestApplicationBuilder ReplaceServiceWithTransient<TService, TImplementation>(
            this ITestApplicationBuilder builder,
            Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            return builder.ConfigureServices(s =>
            {
                s.RemoveAll<TService>();
                s.AddTransient<TService, TImplementation>(factory);
            });
        }
    }
}
