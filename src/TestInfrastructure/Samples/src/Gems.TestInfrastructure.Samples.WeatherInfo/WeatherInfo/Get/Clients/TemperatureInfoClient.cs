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
