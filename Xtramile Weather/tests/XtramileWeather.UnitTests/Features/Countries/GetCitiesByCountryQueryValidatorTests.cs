using XtramileWeather.Application.Features.Countries.GetCitiesByCountry;

namespace XtramileWeather.UnitTests.Features.Countries;

public sealed class GetCitiesByCountryQueryValidatorTests
{
    private readonly GetCitiesByCountryQueryValidator validator = new();

    [Theory]
    [InlineData("ID")]
    [InlineData("us")]
    public async Task Validate_ValidCountryCode_Succeeds(string countryCode)
    {
        var result = await validator.ValidateAsync(new GetCitiesByCountryQuery(countryCode));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("I")]
    [InlineData("IDN")]
    [InlineData("12")]
    [InlineData("I-")]
    public async Task Validate_InvalidCountryCode_FailsForCountryCode(string countryCode)
    {
        var result = await validator.ValidateAsync(new GetCitiesByCountryQuery(countryCode));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == "CountryCode");
    }
}
