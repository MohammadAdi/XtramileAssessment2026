using FluentValidation;

namespace XtramileWeather.Application.Features.Weather.GetWeatherByCity;

public sealed class GetWeatherByCityQueryValidator : AbstractValidator<GetWeatherByCityQuery>
{
    public GetWeatherByCityQueryValidator()
    {
        RuleFor(query => query.CityName)
            .NotEmpty()
            .WithMessage("City name is required.")
            .MaximumLength(100)
            .WithMessage("City name must not exceed 100 characters.")
            .Must(cityName => !string.IsNullOrWhiteSpace(cityName))
            .WithMessage("City name cannot be empty or whitespace.");
    }
}
