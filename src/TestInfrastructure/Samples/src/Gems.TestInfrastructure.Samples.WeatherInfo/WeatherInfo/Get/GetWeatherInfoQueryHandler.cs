﻿using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;
using Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Clients;
using Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get.Dto;

using MediatR;

namespace Gems.TestInfrastructure.Samples.WeatherInfo.WeatherInfo.Get;

[Endpoint("", "GET")]
public class GetWeatherInfoQueryHandler : IRequestHandler<GetWeatherInfoQuery, GetWeatherInfoQueryResponse>
{
    private readonly IUnitOfWorkProvider provider;
    private readonly ITemperatureInfoClient temperatureInfoClient;
    private readonly IPrecipitationInfoClient recipitationInfoClient;

    public GetWeatherInfoQueryHandler(
        IUnitOfWorkProvider provider,
        ITemperatureInfoClient temperatureInfoClient,
        IPrecipitationInfoClient recipitationInfoClient)
    {
        this.provider = provider;
        this.temperatureInfoClient = temperatureInfoClient;
        this.recipitationInfoClient = recipitationInfoClient;
    }

    public async Task<GetWeatherInfoQueryResponse> Handle(GetWeatherInfoQuery request, CancellationToken cancellationToken)
    {
        var towns = await this.provider.GetUnitOfWork("DefaultConnection", cancellationToken)
            .QueryAsync<string>("SELECT town_name FROM public.towns;");
        var response = new GetWeatherInfoQueryResponse
        {
            Items = towns.Select(x => new GetWeatherInfoQueryResponseItem()
            {
                Town = x,
                Temperature = "15 C",
                Precipitation = "Rain",
            }).ToList(),
        };

        if (string.IsNullOrEmpty(request.Mode) || request.Mode.Equals("sync", StringComparison.InvariantCultureIgnoreCase))
        {
            foreach (var item in response.Items)
            {
                item.Temperature = await this.temperatureInfoClient.GetTemperatureAsync(item.Town, cancellationToken);
                item.Precipitation = await this.recipitationInfoClient.GetPrecipitationAsync(item.Town, cancellationToken);
            }
        }
        else if (request.Mode.Equals("async", StringComparison.InvariantCultureIgnoreCase))
        {
            var tasks = new Dictionary<GetWeatherInfoQueryResponseItem, Tuple<Task<string>, Task<string>>>();
            foreach (var item in response.Items)
            {
                var task1 = this.temperatureInfoClient.GetTemperatureAsync(item.Town, cancellationToken);
                var task2 = this.recipitationInfoClient.GetPrecipitationAsync(item.Town, cancellationToken);
                tasks.Add(item, new Tuple<Task<string>, Task<string>>(task1, task2));
            }

            await Task.WhenAll(tasks.Values.SelectMany(x => new Task[] { x.Item1, x.Item2 }).ToArray());

            foreach (var item in response.Items)
            {
                var results = tasks[item];
                item.Temperature = results.Item1.Result;
                item.Precipitation = results.Item2.Result;
            }
        }
        else
        {
            throw new ArgumentException($"Invalid mode \"{request.Mode}\". Valid values: sync, async");
        }

        return response;
    }
}