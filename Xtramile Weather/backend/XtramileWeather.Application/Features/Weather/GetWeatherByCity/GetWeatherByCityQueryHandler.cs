using MediatR;
using XtramileWeather.Application.Abstractions.Services;

namespace XtramileWeather.Application.Features.Weather.GetWeatherByCity;

public sealed class GetWeatherByCityQueryHandler(IWeatherService weatherService)
    : IRequestHandler<GetWeatherByCityQuery, WeatherObservation>
{
    public async Task<WeatherObservation> Handle(
        GetWeatherByCityQuery request,
        CancellationToken cancellationToken)
    {
        var weatherData = await weatherService.GetCurrentWeatherAsync(
            request.CityName,
            cancellationToken);

        // Convert Fahrenheit to Celsius: (F - 32) × 5/9
        var temperatureCelsius = Math.Round((weatherData.TemperatureFahrenheit - 32) * 5 / 9, 1);

        return weatherData with
        {
            TemperatureCelsius = temperatureCelsius
        };
    }
}
