using Microsoft.EntityFrameworkCore;
using XtramileWeather.Application.Countries;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Infrastructure.Persistence.Repositories;

public sealed class CityRepository(WeatherDbContext dbContext) : ICityRepository
{
    public Task<bool> ExistsAsync(int cityId, CancellationToken cancellationToken)
        => dbContext.Cities
            .AsNoTracking()
            .AnyAsync(city => city.Id == cityId, cancellationToken);

    public async Task<IReadOnlyList<City>> ListByCountryCodeAsync(
        string countryCode,
        CancellationToken cancellationToken)
        => await dbContext.Cities
            .AsNoTracking()
            .Where(city => city.CountryCode == countryCode)
            .OrderBy(city => city.Name)
            .ToArrayAsync(cancellationToken);
}
