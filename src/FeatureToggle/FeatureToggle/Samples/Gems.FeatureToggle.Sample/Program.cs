// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.FeatureToggle;
using Gems.FeatureToggle.Sample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureFeatureToggle<TestBackgroundService>(builder.Configuration);

builder.Services.AddHostedService<TestBackgroundService>();

builder.Build().Run();
