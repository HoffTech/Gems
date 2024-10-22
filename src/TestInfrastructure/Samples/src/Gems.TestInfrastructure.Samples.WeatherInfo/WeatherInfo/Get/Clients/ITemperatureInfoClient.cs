// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Clients
{
    public interface ITemperatureInfoClient
    {
        Task<string?> GetTemperatureAsync(string town, CancellationToken cancellationToken);
    }
}
