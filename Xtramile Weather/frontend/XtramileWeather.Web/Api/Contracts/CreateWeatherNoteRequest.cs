namespace XtramileWeather.Web.Api.Contracts;

public sealed record CreateWeatherNoteRequest(int CityId, string Text);
