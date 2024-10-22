// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Dto;

public class GetWeatherInfoQueryResponseItem
{
    public string? Town { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Temperature { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Precipitation { get; set; }
}
