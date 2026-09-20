using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Application.Countries;

/// <summary>
/// Provides city reads required by Application features.
/// </summary>
public interface ICityRepository
{
    Task<bool> ExistsAsync(int cityId, CancellationToken cancellationToken);

    Task<IReadOnlyList<City>> ListByCountryCodeAsync(
        string countryCode,
        CancellationToken cancellationToken);
}
