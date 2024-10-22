// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Clients
{
    public class TemperatureInfoClient : ITemperatureInfoClient
    {
        private readonly HttpClient httpClient;

        public TemperatureInfoClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string?> GetTemperatureAsync(string town, CancellationToken cancellationToken)
        {
            try
            {
                var response = await this.httpClient.GetFromJsonAsync<GetTemperatureInfoResponse>(town, cancellationToken);
                return response!.Temperature;
            }
            catch
            {
                return default;
            }
        }

        private class GetTemperatureInfoResponse
        {
            public string? Temperature { get; set; }
        }
    }
}
