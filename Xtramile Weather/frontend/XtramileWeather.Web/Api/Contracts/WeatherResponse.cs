namespace XtramileWeather.Web.Api.Contracts;

public sealed record WeatherResponse(
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
