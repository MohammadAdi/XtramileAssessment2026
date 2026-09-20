using XtramileWeather.Application.WeatherNotes;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Infrastructure.Persistence.Repositories;

public sealed class WeatherNoteRepository(WeatherDbContext dbContext) : IWeatherNoteRepository
{
    public async Task AddAsync(WeatherNote weatherNote, CancellationToken cancellationToken)
    {
        await dbContext.WeatherNotes.AddAsync(weatherNote, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
