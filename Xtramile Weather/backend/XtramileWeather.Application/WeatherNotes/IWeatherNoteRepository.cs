using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Application.WeatherNotes;

/// <summary>
/// Persists weather notes created for known cities.
/// </summary>
public interface IWeatherNoteRepository
{
    Task AddAsync(WeatherNote weatherNote, CancellationToken cancellationToken);
}
