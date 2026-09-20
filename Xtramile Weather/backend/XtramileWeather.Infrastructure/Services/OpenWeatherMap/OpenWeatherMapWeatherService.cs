using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using XtramileWeather.Application.Abstractions.Services;
using XtramileWeather.Infrastructure.Services.OpenWeatherMap.Models;

namespace XtramileWeather.Infrastructure.Services.OpenWeatherMap;

public sealed class OpenWeatherMapWeatherService(
    HttpClient httpClient,
    IOptions<OpenWeatherMapOptions> options) : IWeatherService
{
    public async Task<WeatherObservation> GetCurrentWeatherAsync(
        string cityName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cityName);

        var apiKey = options.Value.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("The OpenWeatherMap API key is not configured.");
        }

        var requestUri = $"data/2.5/weather?q={Uri.EscapeDataString(cityName)}&units=imperial&appid={Uri.EscapeDataString(apiKey)}";
        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<OpenWeatherMapResponse>(
            cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("The weather provider returned an empty response.");

        // Calculate dew point using Magnus formula
        // First convert Fahrenheit to Celsius for calculation
        var tempCelsius = (payload.Main.Temperature - 32) * 5 / 9;
        var humidity = payload.Main.Humidity;
        
        // Magnus formula constants
        const double a = 17.27;
        const double b = 237.7;
        
        // Calculate dew point in Celsius
        var alpha = ((a * tempCelsius) / (b + tempCelsius)) + Math.Log(humidity / 100.0);
        var dewPointCelsius = (b * alpha) / (a - alpha);
        
        // Convert back to Fahrenheit
        var dewPointFahrenheit = (dewPointCelsius * 9 / 5) + 32;

        return new WeatherObservation(
            payload.Name,
            payload.System.CountryCode,
            DateTimeOffset.FromUnixTimeSeconds(payload.ObservedAtUnixSeconds),
            payload.Main.Temperature,
            tempCelsius,
            dewPointFahrenheit,
            payload.Main.Humidity,
            payload.Wind.Speed,
            payload.Wind.DirectionDegrees,
            payload.Visibility,
            payload.Main.Pressure,
            payload.Weather.FirstOrDefault()?.Main ?? "Unknown");
    }
}
