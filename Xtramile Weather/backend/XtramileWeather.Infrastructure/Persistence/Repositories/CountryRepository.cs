using Microsoft.EntityFrameworkCore;
using XtramileWeather.Application.Countries;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Infrastructure.Persistence.Repositories;

public sealed class CountryRepository(WeatherDbContext dbContext) : ICountryRepository
{
    public async Task<IReadOnlyList<Country>> ListAsync(CancellationToken cancellationToken)
        => await dbContext.Countries
            .AsNoTracking()
            .OrderBy(country => country.Name)
            .ToArrayAsync(cancellationToken);

    public Task<Country?> GetByCodeAsync(
        string countryCode,
        CancellationToken cancellationToken)
        => dbContext.Countries
            .AsNoTracking()
            .SingleOrDefaultAsync(country => country.Code == countryCode, cancellationToken);
}
