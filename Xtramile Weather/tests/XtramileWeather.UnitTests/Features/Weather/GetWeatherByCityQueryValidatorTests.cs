using XtramileWeather.Application.Features.Weather.GetWeatherByCity;

namespace XtramileWeather.UnitTests.Features.Weather;

public sealed class GetWeatherByCityQueryValidatorTests
{
    private readonly GetWeatherByCityQueryValidator _validator = new();

    [Theory]
    [InlineData("Jakarta")]
    [InlineData("New York")]
    [InlineData("São Paulo")]
    [InlineData("a")] // Minimum 1 character
    public async Task Validate_ValidCityName_Succeeds(string cityName)
    {
        // Arrange
        var query = new GetWeatherByCityQuery(cityName);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyCityName_Fails()
    {
        // Arrange
        var query = new GetWeatherByCityQuery("");

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => 
            failure.PropertyName == "CityName" && 
            failure.ErrorMessage.Contains("required", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Validate_WhitespaceCityName_Fails()
    {
        // Arrange
        var query = new GetWeatherByCityQuery("   ");

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => 
            failure.PropertyName == "CityName" && 
            failure.ErrorMessage.Contains("whitespace", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Validate_CityNameExceedsMaxLength_Fails()
    {
        // Arrange
        var cityName = new string('a', 101); // 101 characters
        var query = new GetWeatherByCityQuery(cityName);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => 
            failure.PropertyName == "CityName" && 
            failure.ErrorMessage.Contains("100", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Validate_CityNameAtMaxLength_Succeeds()
    {
        // Arrange
        var cityName = new string('a', 100); // Exactly 100 characters
        var query = new GetWeatherByCityQuery(cityName);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }
}
