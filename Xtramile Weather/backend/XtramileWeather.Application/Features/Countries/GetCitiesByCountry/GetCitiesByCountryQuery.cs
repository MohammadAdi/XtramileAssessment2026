using MediatR;

namespace XtramileWeather.Application.Features.Countries.GetCitiesByCountry;

public sealed record GetCitiesByCountryQuery(string CountryCode)
    : IRequest<CountryCitiesDto?>;
