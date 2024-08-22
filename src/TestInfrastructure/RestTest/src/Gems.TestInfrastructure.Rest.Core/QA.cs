// <copyright file="QA.cs" company="Hoff">
// Copyright (c) Company. All rights reserved.
// </copyright>

using Allure.Net.Commons;

using Gems.TestInfrastructure.Rest.Core.Model;

using Microsoft.Extensions.Logging;

namespace Gems.TestInfrastructure.Rest.Core;

public static class QA
{
    public static async Task<bool> RunAsync(
        QAOptions options,
        CancellationToken cancellationToken = default)
    {
        var lifeCycle = AllureLifecycle.Instance;
        if (options.Allure?.CleanupResultDirectory ?? false)
        {
            lifeCycle.CleanupResultDirectory();
        }

        lifeCycle.StartTestContainer(CreateAllureTestResultContainer(options));

        try
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<TestRunner>();
            var globals = new List<TestScope>();
            if (options.ScopeFileNames != null)
            {
                foreach (var scopeFileInfo in options.ScopeFileNames.Select(fileName => new FileInfo(fileName)))
                {
                    globals.Add(await JsonFile.ReadScopeAsync(scopeFileInfo.FullName));
                }
            }

            var success = true;
            var testRunnerOptions = TestRunnerOptions.CreateDefault();
            testRunnerOptions.HttpClient = options.HttpClient;
            using var testRunner = new TestRunner(testRunnerOptions, logger);
            await foreach (var testCollection in JsonFile.ReadTestCollectionAsync(options.Path, options.Recursive))
            {
                success &= await testRunner.RunAsync(testCollection, globals, cancellationToken);
            }

            return success;
        }
        finally
        {
            lifeCycle.WriteTestContainer();
        }
    }

    private static TestResultContainer CreateAllureTestResultContainer(QAOptions options)
    {
        var qaContainer = new TestResultContainer()
        {
            name = "QA Test Session",
            uuid = Guid.NewGuid().ToString("N"),
            description = $"Path: {options.Path}\n$Scopes:\n{string.Join("\n", options.ScopeFileNames)}",
        };

        return qaContainer;
    }
}
