using MediatR;

namespace XtramileWeather.Application.Features.Countries.GetCountries;

public sealed record GetCountriesQuery : IRequest<IReadOnlyList<CountryDto>>;
