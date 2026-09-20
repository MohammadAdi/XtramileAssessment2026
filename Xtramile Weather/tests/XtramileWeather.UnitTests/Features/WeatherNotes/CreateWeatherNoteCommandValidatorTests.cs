using FluentValidation.TestHelper;
using XtramileWeather.Application.Features.WeatherNotes.CreateWeatherNote;
using XtramileWeather.Domain.Entities;

namespace XtramileWeather.UnitTests.Features.WeatherNotes;

public sealed class CreateWeatherNoteCommandValidatorTests
{
    private readonly CreateWeatherNoteCommandValidator validator = new();

    [Fact]
    public void Validate_AcceptsValidCommand()
    {
        var result = validator.TestValidate(new CreateWeatherNoteCommand(4, "Rain expected."));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_RejectsNonPositiveCityIdentifier()
    {
        var result = validator.TestValidate(new CreateWeatherNoteCommand(0, "Rain expected."));

        result.ShouldHaveValidationErrorFor(command => command.CityId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_RejectsBlankText(string text)
    {
        var result = validator.TestValidate(new CreateWeatherNoteCommand(4, text));

        result.ShouldHaveValidationErrorFor(command => command.Text);
    }

    [Fact]
    public void Validate_AcceptsTextAtMaximumLength()
    {
        var result = validator.TestValidate(
            new CreateWeatherNoteCommand(4, new string('a', WeatherNote.MaximumTextLength)));

        result.ShouldNotHaveValidationErrorFor(command => command.Text);
    }

    [Fact]
    public void Validate_RejectsTextOverMaximumLength()
    {
        var result = validator.TestValidate(
            new CreateWeatherNoteCommand(4, new string('a', WeatherNote.MaximumTextLength + 1)));

        result.ShouldHaveValidationErrorFor(command => command.Text);
    }
}
