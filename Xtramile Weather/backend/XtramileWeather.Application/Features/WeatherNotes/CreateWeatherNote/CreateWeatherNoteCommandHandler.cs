using MediatR;
using XtramileWeather.Application.Countries;
using XtramileWeather.Application.WeatherNotes;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Application.Features.WeatherNotes.CreateWeatherNote;

public sealed class CreateWeatherNoteCommandHandler(
    ICityRepository cityRepository,
    IWeatherNoteRepository weatherNoteRepository)
    : IRequestHandler<CreateWeatherNoteCommand, CreateWeatherNoteResponse?>
{
    public async Task<CreateWeatherNoteResponse?> Handle(
        CreateWeatherNoteCommand request,
        CancellationToken cancellationToken)
    {
        if (!await cityRepository.ExistsAsync(request.CityId, cancellationToken))
        {
            return null;
        }

        var weatherNote = new WeatherNote(request.CityId, request.Text, DateTimeOffset.UtcNow);
        await weatherNoteRepository.AddAsync(weatherNote, cancellationToken);

        return new CreateWeatherNoteResponse(weatherNote.Id);
    }
}
