namespace XtramileWeather.Infrastructure.Services.OpenWeatherMap;

public sealed class OpenWeatherMapOptions
{
    public const string SectionName = "OpenWeatherMap";

    public string BaseUrl { get; init; } = string.Empty;

    public string ApiKey { get; init; } = string.Empty;
}
