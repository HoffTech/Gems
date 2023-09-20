// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Settings.Gitlab;

using Hangfire;
using Hangfire.MemoryStorage;

using HangfireDynamicJobs;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(c =>
{
    // Configure from Gitlab
    c.AddGitlabConfiguration();
});

builder.Services.AddGitlabConfigurationUpdater(c => c
        .UpdateInterval(TimeSpan.FromSeconds(30))
        .SetValueChangedHandler((sp, name, newValue, oldValue) =>
        {
            if (name.Equals("MyJobSchedule"))
            {
                var manager = sp.GetRequiredService<IRecurringJobManager>();
                manager.AddOrUpdate(
                    "MyJob",
                    () => Console.WriteLine("Hello!"),
                    newValue);
            }
        }));

// Add services to the container.
builder.Services.AddHangfire(config => config
    .UseRecommendedSerializerSettings()
    .UseMemoryStorage());

builder.Services.AddHangfireServer();

builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard("/dashboard", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthFilter() },
        IgnoreAntiforgeryToken = true,
    });
});

// Add job on startup.
RecurringJob.AddOrUpdate(
    "MyJob",
    () => Console.WriteLine("Hello!"),
    app.Configuration.GetValue<string>("MyJobSchedule"));

app.Run();
