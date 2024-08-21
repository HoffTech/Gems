using System.Runtime.CompilerServices;

using Gems.TestInfrastructure.RestTest.Model;

using Newtonsoft.Json;

namespace Gems.TestInfrastructure.RestTest
{
    public static class JsonFile
    {
        public static async IAsyncEnumerable<TestCollection> ReadTestCollectionAsync(
            string path,
            bool recursive,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (File.Exists(path))
            {
                yield return await ReadTestCollectionFileAsync(path, cancellationToken);
            }
            else if (Directory.Exists(path))
            {
                var options = new EnumerationOptions();
                options.RecurseSubdirectories = recursive;
                foreach (var fileName in Directory.EnumerateFiles(path, "*.json", options))
                {
                    yield return await ReadTestCollectionFileAsync(fileName, cancellationToken);
                }
            }
            else
            {
                var searchPattern = Path.GetFileName(path);
                var directory = Path.GetDirectoryName(path);
                var options = new EnumerationOptions();
                options.RecurseSubdirectories = recursive;
                foreach (var fileName in Directory.EnumerateFiles(directory ?? ".", searchPattern ?? "*.json", options))
                {
                    yield return await ReadTestCollectionFileAsync(fileName, cancellationToken);
                }
            }
        }

        public static async Task<TestCollection> ReadTestCollectionFileAsync(
            string path,
            CancellationToken cancellationToken = default)
        {
            var collectionJson = JsonConvert.DeserializeObject<TestCollectionJson>(
                await File.ReadAllTextAsync(path, cancellationToken));
            return JsonMapper.Map(collectionJson);
        }

        public static async Task<Test> ReadTestAsync(string path)
        {
            var testJson = JsonConvert.DeserializeObject<TestJson>(
                await File.ReadAllTextAsync(path));
            return JsonMapper.Map(testJson);
        }

        public static async Task<TestScope> ReadScopeAsync(string path)
        {
            var scopeJson = JsonConvert.DeserializeObject<TestScopeJson>(
                await File.ReadAllTextAsync(path));
            return JsonMapper.Map(scopeJson);
        }
    }
}
