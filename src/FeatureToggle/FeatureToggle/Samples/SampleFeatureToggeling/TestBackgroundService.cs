// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.FeatureToggle;

namespace SampleFeatureToggeling;

public class TestBackgroundService(
    ILogger<TestBackgroundService> logger,
    TestToggles testToggles,
    IFeatureToggleService featureToggleService)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            if (!testToggles.EnableFeatureTesting)
            {
                logger.LogInformation("Feature testing disabled. Please add 'enable_feature_testing' toggle and set it enabled.");
                continue;
            }

            var enabledCount = 0;
            var totalUsersCount = 1000;
            for (var i = 0; i < totalUsersCount; i++)
            {
                if (featureToggleService.IsEnabled(
                        "rolling_feature",
                        new Dictionary<string, string> { ["userId"] = Guid.NewGuid().ToString("N") }))
                {
                    enabledCount++;
                }
            }

            var featureEnabledRatio = enabledCount * 100.0 / totalUsersCount;
            logger.LogInformation($"Feature enabled for {featureEnabledRatio:F1} % of users");
            logger.LogInformation("Add toggle 'rolling_feature' and  set percent rolout strategy, to play.");
        }
    }
}

[FeatureToggles]
public class TestToggles
{
    public bool EnableFeatureTesting { get; set; }
}
