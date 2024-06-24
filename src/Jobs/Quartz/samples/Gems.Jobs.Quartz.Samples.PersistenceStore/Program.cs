// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Jobs.Quartz.Samples.PersistenceStore;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureWebHostDefaults(b => b.UseStartup<Startup>());
builder.Build().Run();
