namespace XtramileWeather.Application.Features.Countries.GetCitiesByCountry;

public sealed record CityDto(
    int Id,
    string Name,
    decimal Latitude,
    decimal Longitude);
