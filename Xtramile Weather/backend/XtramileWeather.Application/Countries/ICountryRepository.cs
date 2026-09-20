using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Application.Countries;

/// <summary>
/// Provides country reads required by Application features.
/// </summary>
public interface ICountryRepository
{
    Task<IReadOnlyList<Country>> ListAsync(CancellationToken cancellationToken);

    Task<Country?> GetByCodeAsync(string countryCode, CancellationToken cancellationToken);
}
