namespace XtramileWeather.Web.Api.Contracts;

public sealed record CountryCitiesResponse(
    string CountryCode,
    string CountryName,
    IReadOnlyList<CityResponse> Cities);
