using FluentValidation;

namespace XtramileWeather.Application.Features.Countries.GetCitiesByCountry;

public sealed class GetCitiesByCountryQueryValidator : AbstractValidator<GetCitiesByCountryQuery>
{
    public GetCitiesByCountryQueryValidator()
    {
        RuleFor(query => query.CountryCode)
            .NotEmpty()
            .WithMessage("Country code is required.")
            .Length(2)
            .WithMessage("Country code must contain exactly two letters.")
            .Matches("^[A-Za-z]{2}$")
            .WithMessage("Country code must be a two-letter ISO 3166-1 alpha-2 code.");
    }
}
