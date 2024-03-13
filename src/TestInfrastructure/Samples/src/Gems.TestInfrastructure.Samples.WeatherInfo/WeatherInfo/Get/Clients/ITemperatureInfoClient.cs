namespace Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Clients
{
    public interface ITemperatureInfoClient
    {
        Task<string?> GetTemperatureAsync(string town, CancellationToken cancellationToken);
    }
}