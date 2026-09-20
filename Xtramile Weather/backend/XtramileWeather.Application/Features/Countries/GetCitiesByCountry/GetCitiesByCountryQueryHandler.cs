using MediatR;
using XtramileWeather.Application.Countries;

namespace XtramileWeather.Application.Features.Countries.GetCitiesByCountry;

public sealed class GetCitiesByCountryQueryHandler(
    ICountryRepository countryRepository,
    ICityRepository cityRepository)
    : IRequestHandler<GetCitiesByCountryQuery, CountryCitiesDto?>
{
    public async Task<CountryCitiesDto?> Handle(
        GetCitiesByCountryQuery request,
        CancellationToken cancellationToken)
    {
        var normalizedCountryCode = request.CountryCode.ToUpperInvariant();
        var country = await countryRepository.GetByCodeAsync(
            normalizedCountryCode,
            cancellationToken);

        if (country is null)
        {
            return null;
        }

        var cities = await cityRepository.ListByCountryCodeAsync(
            normalizedCountryCode,
            cancellationToken);
        var cityDtos = cities
            .Select(city => new CityDto(city.Id, city.Name, city.Latitude, city.Longitude))
            .ToArray();

        return new CountryCitiesDto(country.Code, country.Name, cityDtos);
    }
}
