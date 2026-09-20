using MediatR;

namespace XtramileWeather.Application.Features.WeatherNotes.CreateWeatherNote;

public sealed record CreateWeatherNoteCommand(int CityId, string Text)
    : IRequest<CreateWeatherNoteResponse?>;
