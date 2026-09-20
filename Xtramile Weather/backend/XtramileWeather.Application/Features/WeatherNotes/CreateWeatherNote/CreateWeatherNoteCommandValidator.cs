using FluentValidation;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.Application.Features.WeatherNotes.CreateWeatherNote;

public sealed class CreateWeatherNoteCommandValidator : AbstractValidator<CreateWeatherNoteCommand>
{
    public CreateWeatherNoteCommandValidator()
    {
        RuleFor(command => command.CityId).GreaterThan(0);
        RuleFor(command => command.Text)
            .NotEmpty()
            .MaximumLength(WeatherNote.MaximumTextLength);
    }
}
