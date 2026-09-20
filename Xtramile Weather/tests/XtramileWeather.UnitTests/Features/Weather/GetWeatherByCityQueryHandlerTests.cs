using NSubstitute;
using XtramileWeather.Application.Abstractions.Services;
using XtramileWeather.Application.Features.Weather.GetWeatherByCity;

namespace XtramileWeather.UnitTests.Features.Weather;

public sealed class GetWeatherByCityQueryHandlerTests
{
    private readonly IWeatherService _weatherService;
    private readonly GetWeatherByCityQueryHandler _handler;

    public GetWeatherByCityQueryHandlerTests()
    {
        _weatherService = Substitute.For<IWeatherService>();
        _handler = new GetWeatherByCityQueryHandler(_weatherService);
    }

    [Fact]
    public async Task Handle_ValidCityName_ReturnsWeatherWithConvertedTemperature()
    {
        // Arrange
        var query = new GetWeatherByCityQuery("Jakarta");
        var serviceResponse = new WeatherObservation(
            City: "Jakarta",
            CountryCode: "ID",
            ObservedAtUtc: DateTimeOffset.UtcNow,
            TemperatureFahrenheit: 86.0,
            TemperatureCelsius: 0, // Will be calculated by handler
            DewPointFahrenheit: 75.0,
            RelativeHumidityPercent: 80,
            WindSpeed: 10.5,
            WindDirectionDegrees: 180,
            VisibilityMeters: 10000,
            PressureHpa: 1013,
            Condition: "Clear"
        );

        _weatherService.GetCurrentWeatherAsync(query.CityName, Arg.Any<CancellationToken>())
            .Returns(serviceResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jakarta", result.City);
        Assert.Equal(86.0, result.TemperatureFahrenheit);
        Assert.Equal(30.0, result.TemperatureCelsius, precision: 1); // (86-32)*5/9 = 30°C
    }

    [Theory]
    [InlineData(32.0, 0.0)]     // Freezing point
    [InlineData(212.0, 100.0)]  // Boiling point
    [InlineData(-40.0, -40.0)]  // Same in both scales
    [InlineData(0.0, -17.8)]    // Below freezing
    [InlineData(98.6, 37.0)]    // Body temperature
    public async Task Handle_VariousTemperatures_ConvertsCorrectly(double fahrenheit, double expectedCelsius)
    {
        // Arrange
        var query = new GetWeatherByCityQuery("TestCity");
        var serviceResponse = new WeatherObservation(
            City: "TestCity",
            CountryCode: "XX",
            ObservedAtUtc: DateTimeOffset.UtcNow,
            TemperatureFahrenheit: fahrenheit,
            TemperatureCelsius: 0,
            DewPointFahrenheit: 50.0,
            RelativeHumidityPercent: 50,
            WindSpeed: 5.0,
            WindDirectionDegrees: 90,
            VisibilityMeters: 10000,
            PressureHpa: 1013,
            Condition: "Clear"
        );

        _weatherService.GetCurrentWeatherAsync(query.CityName, Arg.Any<CancellationToken>())
            .Returns(serviceResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(expectedCelsius, result.TemperatureCelsius, precision: 1);
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        var query = new GetWeatherByCityQuery("NonExistentCity");
        _weatherService.GetCurrentWeatherAsync(query.CityName, Arg.Any<CancellationToken>())
            .Returns<WeatherObservation>(x => throw new HttpRequestException("City not found"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_PropagatesCancellation()
    {
        // Arrange
        var query = new GetWeatherByCityQuery("Jakarta");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _weatherService.GetCurrentWeatherAsync(query.CityName, Arg.Any<CancellationToken>())
            .Returns<WeatherObservation>(x => throw new TaskCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => 
            _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_PreservesAllWeatherData()
    {
        // Arrange
        var query = new GetWeatherByCityQuery("Sydney");
        var observedTime = new DateTimeOffset(2026, 9, 19, 10, 0, 0, TimeSpan.Zero);
        var serviceResponse = new WeatherObservation(
            City: "Sydney",
            CountryCode: "AU",
            ObservedAtUtc: observedTime,
            TemperatureFahrenheit: 74.5,
            TemperatureCelsius: 0,
            DewPointFahrenheit: 65.0,
            RelativeHumidityPercent: 75,
            WindSpeed: 12.3,
            WindDirectionDegrees: 270,
            VisibilityMeters: 15000,
            PressureHpa: 1020,
            Condition: "Partly Cloudy"
        );

        _weatherService.GetCurrentWeatherAsync(query.CityName, Arg.Any<CancellationToken>())
            .Returns(serviceResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal("Sydney", result.City);
        Assert.Equal("AU", result.CountryCode);
        Assert.Equal(observedTime, result.ObservedAtUtc);
        Assert.Equal(74.5, result.TemperatureFahrenheit);
        Assert.Equal(65.0, result.DewPointFahrenheit);
        Assert.Equal(75, result.RelativeHumidityPercent);
        Assert.Equal(12.3, result.WindSpeed);
        Assert.Equal(270, result.WindDirectionDegrees);
        Assert.Equal(15000, result.VisibilityMeters);
        Assert.Equal(1020, result.PressureHpa);
        Assert.Equal("Partly Cloudy", result.Condition);
    }
}
