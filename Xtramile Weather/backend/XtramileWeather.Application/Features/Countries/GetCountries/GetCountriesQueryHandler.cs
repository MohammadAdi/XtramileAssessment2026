using MediatR;
using XtramileWeather.Application.Countries;

namespace XtramileWeather.Application.Features.Countries.GetCountries;

public sealed class GetCountriesQueryHandler(ICountryRepository countryRepository)
    : IRequestHandler<GetCountriesQuery, IReadOnlyList<CountryDto>>
{
    public async Task<IReadOnlyList<CountryDto>> Handle(
        GetCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var countries = await countryRepository.ListAsync(cancellationToken);

        return countries
            .Select(country => new CountryDto(country.Code, country.Name))
            .ToArray();
    }
}
