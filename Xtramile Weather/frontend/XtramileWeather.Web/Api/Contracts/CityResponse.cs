namespace XtramileWeather.Web.Api.Contracts;

public sealed record CityResponse(int Id, string Name, decimal Latitude, decimal Longitude);
