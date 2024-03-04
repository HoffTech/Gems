using Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Dto;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get;

public class GetWeatherInfoQuery: IRequest<GetWeatherInfoQueryResponse>
{
    [FromQuery]
    public string Mode { get; set; }
}