using Gems.Mvc;
using Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Clients;

namespace Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get
{
    public class GetWeatherInfoServicesConfiguration : IServicesConfiguration
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ITemperatureInfoClient, TemperatureInfoClient>(httpClient =>
                httpClient.BaseAddress = new Uri(configuration.GetConnectionString("TemperatureInfo")));
            services.AddHttpClient<IPrecipitationInfoClient, PrecipitationInfoClient>(httpClient =>
                httpClient.BaseAddress = new Uri(configuration.GetConnectionString("PrecipitationInfo")));
        }
    }
}
