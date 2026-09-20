namespace XtramileWeather.Api.Contracts.WeatherNotes;

public sealed record CreateWeatherNoteRequest(int CityId, string Text);
