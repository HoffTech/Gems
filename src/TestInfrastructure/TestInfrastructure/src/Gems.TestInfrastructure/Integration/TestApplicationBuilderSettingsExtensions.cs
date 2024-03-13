using System.Text.RegularExpressions;

using Gems.TestInfrastructure.Utils;

namespace Gems.TestInfrastructure.Integration
{
    public static class TestApplicationBuilderSettingsExtensions
    {
        public static ITestApplicationBuilder UseSetting<T>(this ITestApplicationBuilder builder, string name, T value)
        {
            foreach (var kv in SettingsHelper.FlattenObject(value, name))
            {
                builder.UseSetting(kv.Key, kv.Value);
            }

            return builder;
        }

        public static ITestApplicationBuilder ReplaceSetting(
            this ITestApplicationBuilder builder,
            string path,
            string value)
        {
            builder.ConfigureAppConfiguration((ctx, cb) =>
            {
                ctx.Configuration
                    .FlattenConfiguration()
                    .Where(kv => kv.Path == path)
                    .ToList()
                    .ForEach(s => s.Value = value);
            });

            return builder;
        }

        public static ITestApplicationBuilder ReplaceSetting(
            this ITestApplicationBuilder builder,
            string path,
            StringComparison stringComparison,
            string value)
        {
            builder.ConfigureAppConfiguration((ctx, cb) =>
            {
                ctx.Configuration
                    .FlattenConfiguration()
                    .Where(kv => kv.Path.Equals(path, stringComparison))
                    .ToList()
                    .ForEach(s => s.Value = value);
            });

            return builder;
        }

        public static ITestApplicationBuilder ReplaceSetting(
            this ITestApplicationBuilder builder,
            string path,
            Func<string, string> valueFactory)
        {
            builder.ConfigureAppConfiguration((ctx, cb) =>
            {
                ctx.Configuration
                    .FlattenConfiguration()
                    .Where(kv => kv.Path.Equals(path))
                    .ToList()
                    .ForEach(s => s.Value = valueFactory(s.Value));
            });

            return builder;
        }

        public static ITestApplicationBuilder ReplaceSetting(
            this ITestApplicationBuilder builder,
            string path,
            StringComparison stringComparison,
            Func<string, string> valueFactory)
        {
            builder.ConfigureAppConfiguration((ctx, cb) =>
            {
                ctx.Configuration
                    .FlattenConfiguration()
                    .Where(kv => kv.Path.Equals(path, stringComparison))
                    .ToList()
                    .ForEach(s => s.Value = valueFactory(s.Value));
            });

            return builder;
        }

        public static ITestApplicationBuilder ReplaceSetting(
            this ITestApplicationBuilder builder,
            Func<string, bool> pathMatcher,
            Func<string, string> valueFactory)
        {
            builder.ConfigureAppConfiguration((ctx, cb) =>
            {
                ctx.Configuration
                    .FlattenConfiguration()
                    .Where(kv => pathMatcher(kv.Path))
                    .ToList()
                    .ForEach(s => s.Value = valueFactory(s.Value));
            });

            return builder;
        }

        public static ITestApplicationBuilder ReplaceSetting(
            this ITestApplicationBuilder builder,
            Regex pathRx,
            Func<string, string> valueFactory)
        {
            builder.ConfigureAppConfiguration((ctx, cb) =>
            {
                ctx.Configuration
                    .FlattenConfiguration()
                    .Where(kv => pathRx.IsMatch(kv.Path))
                    .ToList()
                    .ForEach(s => s.Value = valueFactory(s.Value));
            });

            return builder;
        }

        public static ITestApplicationBuilder ReplaceSetting(
            this ITestApplicationBuilder builder,
            Regex pathRx,
            string value)
        {
            builder.ConfigureAppConfiguration((ctx, cb) =>
            {
                ctx.Configuration
                    .FlattenConfiguration()
                    .Where(kv => pathRx.IsMatch(kv.Path))
                    .ToList()
                    .ForEach(s => s.Value = value);
            });

            return builder;
        }
    }
}
