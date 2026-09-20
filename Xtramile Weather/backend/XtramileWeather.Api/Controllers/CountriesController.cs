using MediatR;
using Microsoft.AspNetCore.Mvc;
using XtramileWeather.Application.Features.Countries.GetCitiesByCountry;
using XtramileWeather.Application.Features.Countries.GetCountries;

namespace XtramileWeather.Api.Controllers;

[ApiController]
[Route("api/countries")]
public sealed class CountriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<CountryDto>> GetCountries(CancellationToken cancellationToken)
        => sender.Send(new GetCountriesQuery(), cancellationToken);

    [HttpGet("{countryCode}/cities")]
    public async Task<ActionResult<CountryCitiesDto>> GetCities(
        string countryCode,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetCitiesByCountryQuery(countryCode), cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }
}
