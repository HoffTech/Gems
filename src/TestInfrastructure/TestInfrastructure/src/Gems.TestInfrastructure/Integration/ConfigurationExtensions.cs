using Microsoft.Extensions.Configuration;

namespace Gems.TestInfrastructure.Integration
{
    public static class ConfigurationExtensions
    {
        public static IEnumerable<IConfigurationSection> FlattenConfiguration(this IConfiguration configuration)
        {
            foreach (var child in configuration.GetChildren())
            {
                yield return child;
                foreach (var subSection in child.FlattenConfiguration())
                {
                    yield return subSection;
                }
            }
        }

        public static IEnumerable<IConfigurationSection> FlattenConfiguration(this IConfigurationSection section)
        {
            foreach (var child in section.GetChildren())
            {
                yield return child;
                foreach (var subSection in child.FlattenConfiguration())
                {
                    yield return subSection;
                }
            }
        }
    }
}
