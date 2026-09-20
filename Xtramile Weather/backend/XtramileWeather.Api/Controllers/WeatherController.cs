using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using XtramileWeather.Api.Contracts.WeatherNotes;
using XtramileWeather.Application.Abstractions.Services;
using XtramileWeather.Application.Features.Weather.GetWeatherByCity;
using XtramileWeather.Application.Features.WeatherNotes.CreateWeatherNote;

namespace XtramileWeather.Api.Controllers;

[ApiController]
[Route("api/weather")]
public sealed class WeatherController(ISender sender) : ControllerBase
{
    [HttpPost("notes")]
    public async Task<ActionResult<CreateWeatherNoteResponse>> CreateWeatherNote(
        CreateWeatherNoteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateWeatherNoteCommand(request.CityId, request.Text),
            cancellationToken);

        if (result is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "City not found",
                Detail = $"City {request.CityId} does not exist."
            });
        }

        return Created($"/api/weather/notes/{result.Id}", result);
    }

    [HttpGet("{cityName}")]
    public async Task<ActionResult<WeatherObservation>> GetWeatherByCity(
        string cityName,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new GetWeatherByCityQuery(cityName), cancellationToken);
            return Ok(result);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound(new ProblemDetails
            {
                Status = 404,
                Title = "City not found",
                Detail = ex.Message
            });
        }
    }
}
