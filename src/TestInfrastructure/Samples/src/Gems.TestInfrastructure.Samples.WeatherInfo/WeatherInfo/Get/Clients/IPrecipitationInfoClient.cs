namespace Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Clients
{
    public interface IPrecipitationInfoClient
    {
        Task<string?> GetPrecipitationAsync(string town, CancellationToken cancellationToken);
    }
}