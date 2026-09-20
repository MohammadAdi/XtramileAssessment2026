using System.Text.Json.Serialization;

namespace XtramileWeather.Infrastructure.Services.OpenWeatherMap.Models;

internal sealed record OpenWeatherMapResponse(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("dt")] long ObservedAtUnixSeconds,
    [property: JsonPropertyName("sys")] OpenWeatherMapSystem System,
    [property: JsonPropertyName("main")] OpenWeatherMapMain Main,
    [property: JsonPropertyName("wind")] OpenWeatherMapWind Wind,
    [property: JsonPropertyName("visibility")] int Visibility,
    [property: JsonPropertyName("weather")] IReadOnlyList<OpenWeatherMapCondition> Weather);

internal sealed record OpenWeatherMapSystem(
    [property: JsonPropertyName("country")] string CountryCode);

internal sealed record OpenWeatherMapMain(
    [property: JsonPropertyName("temp")] double Temperature,
    [property: JsonPropertyName("humidity")] int Humidity,
    [property: JsonPropertyName("pressure")] int Pressure);

internal sealed record OpenWeatherMapWind(
    [property: JsonPropertyName("speed")] double Speed,
    [property: JsonPropertyName("deg")] int DirectionDegrees);

internal sealed record OpenWeatherMapCondition(
    [property: JsonPropertyName("main")] string Main);
