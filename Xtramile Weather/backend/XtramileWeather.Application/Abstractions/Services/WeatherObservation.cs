namespace XtramileWeather.Application.Abstractions.Services;

/// <summary>Represents provider-neutral current weather data with temperature in both Fahrenheit and Celsius.</summary>
public sealed record WeatherObservation(
    string City,
    string CountryCode,
    DateTimeOffset ObservedAtUtc,
    double TemperatureFahrenheit,
    double TemperatureCelsius,
    double DewPointFahrenheit,
    int RelativeHumidityPercent,
    double WindSpeed,
    int WindDirectionDegrees,
    int VisibilityMeters,
    int PressureHpa,
    string Condition);
