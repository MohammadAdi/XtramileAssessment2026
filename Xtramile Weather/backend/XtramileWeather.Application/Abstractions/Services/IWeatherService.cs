namespace XtramileWeather.Application.Abstractions.Services;

/// <summary>Retrieves current weather from an external provider.</summary>
public interface IWeatherService
{
    Task<WeatherObservation> GetCurrentWeatherAsync(
        string cityName,
        CancellationToken cancellationToken);
}
