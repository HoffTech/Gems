// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace DynamicLoggingLevel
{
    public class LoggingBackgroundService : BackgroundService
    {
        private readonly ILogger<LoggingBackgroundService> logger;

        public LoggingBackgroundService(ILogger<LoggingBackgroundService> logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                this.logger.LogInformation("Sample information message");
                await Task.Delay(1000, stoppingToken);
                this.logger.LogError("Sample error message");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
