// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using DynamicLoggingLevel;

using Gems.Settings.Gitlab;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(c =>
{
    // Configure from Gitlab
    c.AddGitlabConfiguration();
});

builder.Services.AddGitlabConfigurationUpdater(c => c.UpdateInterval(TimeSpan.FromSeconds(30)));

builder.Services.AddHostedService<LoggingBackgroundService>();

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGet("/hello", () => "Hello world!");

app.Run();
