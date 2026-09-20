namespace XtramileWeather.Application.Features.Countries.GetCitiesByCountry;

public sealed record CountryCitiesDto(
    string CountryCode,
    string CountryName,
    IReadOnlyList<CityDto> Cities);
