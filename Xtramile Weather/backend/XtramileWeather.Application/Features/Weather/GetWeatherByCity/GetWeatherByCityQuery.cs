using MediatR;
using XtramileWeather.Application.Abstractions.Services;

namespace XtramileWeather.Application.Features.Weather.GetWeatherByCity;

public sealed record GetWeatherByCityQuery(string CityName)
    : IRequest<WeatherObservation>;
