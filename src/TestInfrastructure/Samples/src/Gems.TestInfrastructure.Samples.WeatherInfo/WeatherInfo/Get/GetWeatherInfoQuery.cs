// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Dto;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get;

public class GetWeatherInfoQuery : IRequest<GetWeatherInfoQueryResponse>
{
    [FromQuery]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Mode { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
