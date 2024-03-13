// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Clients
{
    public class PrecipitationInfoClient : IPrecipitationInfoClient
    {
        private readonly HttpClient httpClient;

        public PrecipitationInfoClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string?> GetPrecipitationAsync(string town, CancellationToken cancellationToken)
        {
            try
            {
                var response = await this.httpClient.GetFromJsonAsync<PrecipitationInfoResponse>(town, cancellationToken);
                return response!.Precipitation;
            }
            catch
            {
                return default;
            }
        }

        private class PrecipitationInfoResponse
        {
            public string? Precipitation { get; set; }
        }
    }
}
