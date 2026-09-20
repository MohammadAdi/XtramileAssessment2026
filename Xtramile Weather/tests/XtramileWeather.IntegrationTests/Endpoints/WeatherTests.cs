using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using XtramileWeather.Application.Abstractions.Services;

namespace XtramileWeather.IntegrationTests.Endpoints;

public sealed class WeatherTests : IClassFixture<WeatherTests.WeatherWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IWeatherService _mockWeatherService;

    public WeatherTests(WeatherWebApplicationFactory factory)
    {
        _mockWeatherService = factory.MockWeatherService;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetWeatherByCity_ValidCity_ReturnsWeatherData()
    {
        // Arrange
        var expectedWeather = new WeatherObservation(
            City: "Jakarta",
            CountryCode: "ID",
            ObservedAtUtc: new DateTimeOffset(2026, 9, 19, 10, 0, 0, TimeSpan.Zero),
            TemperatureFahrenheit: 86.0,
            TemperatureCelsius: 0, // Will be calculated
            DewPointFahrenheit: 75.0,
            RelativeHumidityPercent: 80,
            WindSpeed: 10.5,
            WindDirectionDegrees: 180,
            VisibilityMeters: 10000,
            PressureHpa: 1013,
            Condition: "Clear"
        );

        _mockWeatherService.GetCurrentWeatherAsync("Jakarta", Arg.Any<CancellationToken>())
            .Returns(expectedWeather);

        // Act
        var weather = await _client.GetFromJsonAsync<WeatherPayload>("/api/weather/Jakarta");

        // Assert
        Assert.NotNull(weather);
        Assert.Equal("Jakarta", weather.City);
        Assert.Equal("ID", weather.CountryCode);
        Assert.Equal(86.0, weather.TemperatureFahrenheit);
        Assert.Equal(30.0, weather.TemperatureCelsius, precision: 1); // (86-32)*5/9 = 30°C
        Assert.Equal(75.0, weather.DewPointFahrenheit);
        Assert.Equal(80, weather.RelativeHumidityPercent);
        Assert.Equal(10.5, weather.WindSpeed);
        Assert.Equal(180, weather.WindDirectionDegrees);
        Assert.Equal(10000, weather.VisibilityMeters);
        Assert.Equal(1013, weather.PressureHpa);
        Assert.Equal("Clear", weather.Condition);
    }

    [Theory]
    [InlineData(32.0, 0.0)]     // Freezing point
    [InlineData(212.0, 100.0)]  // Boiling point
    [InlineData(-40.0, -40.0)]  // Same in both scales
    public async Task GetWeatherByCity_VariousTemperatures_ConvertsCorrectly(double fahrenheit, double expectedCelsius)
    {
        // Arrange
        var weather = new WeatherObservation(
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

        _mockWeatherService.GetCurrentWeatherAsync("TestCity", Arg.Any<CancellationToken>())
            .Returns(weather);

        // Act
        var result = await _client.GetFromJsonAsync<WeatherPayload>("/api/weather/TestCity");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCelsius, result.TemperatureCelsius, precision: 1);
    }

    [Fact]
    public async Task GetWeatherByCity_WhitespaceOnlyCityName_ReturnsBadRequest()
    {
        // Arrange - whitespace passes routing but fails validation
        var whitespaceCityName = "   ";
        
        // Act
        using var response = await _client.GetAsync($"/api/weather/{Uri.EscapeDataString(whitespaceCityName)}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetWeatherByCity_CityNameTooLong_ReturnsBadRequest()
    {
        // Arrange
        var longCityName = new string('a', 101);

        // Act
        using var response = await _client.GetAsync($"/api/weather/{longCityName}");
        using var problemDetails = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(400, problemDetails.RootElement.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task GetWeatherByCity_ServiceThrowsNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockWeatherService.GetCurrentWeatherAsync("NonExistentCity", Arg.Any<CancellationToken>())
            .Returns<WeatherObservation>(x => throw new HttpRequestException("Not found", null, HttpStatusCode.NotFound));

        // Act
        using var response = await _client.GetAsync("/api/weather/NonExistentCity");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetWeatherByCity_CityWithSpecialCharacters_WorksCorrectly()
    {
        // Arrange
        var cityName = "São Paulo";
        var weather = new WeatherObservation(
            City: cityName,
            CountryCode: "BR",
            ObservedAtUtc: DateTimeOffset.UtcNow,
            TemperatureFahrenheit: 77.0,
            TemperatureCelsius: 0,
            DewPointFahrenheit: 65.0,
            RelativeHumidityPercent: 70,
            WindSpeed: 8.0,
            WindDirectionDegrees: 90,
            VisibilityMeters: 10000,
            PressureHpa: 1015,
            Condition: "Cloudy"
        );

        _mockWeatherService.GetCurrentWeatherAsync(cityName, Arg.Any<CancellationToken>())
            .Returns(weather);

        // Act
        var result = await _client.GetFromJsonAsync<WeatherPayload>($"/api/weather/{Uri.EscapeDataString(cityName)}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cityName, result.City);
    }

    private sealed record WeatherPayload(
        string City,
        string CountryCode,
        DateTimeOffset ObservedAtUtc,
        double TemperatureFahrenheit,
        double TemperatureCelsius,
        double DewPointFahrenheit,
        int RelativeHumidityPercent,
        double WindSpeed,
        int WindDirectionDegrees,
        int VisibilityMeters,
        int PressureHpa,
        string Condition);

    public sealed class WeatherWebApplicationFactory : WebApplicationFactory<Program>
    {
        public IWeatherService MockWeatherService { get; } = Substitute.For<IWeatherService>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace IWeatherService with mock
                services.RemoveAll<IWeatherService>();
                services.AddSingleton(MockWeatherService);

                // Use in-memory database for other dependencies
                services.RemoveAll<Microsoft.EntityFrameworkCore.DbContextOptions<XtramileWeather.Infrastructure.Persistence.WeatherDbContext>>();
                services.RemoveAll<XtramileWeather.Infrastructure.Persistence.WeatherDbContext>();
                services.AddSingleton(_ =>
                {
                    var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
                    connection.Open();
                    return connection;
                });
                services.AddDbContext<XtramileWeather.Infrastructure.Persistence.WeatherDbContext>((serviceProvider, options) =>
                    options.UseSqlite(serviceProvider.GetRequiredService<Microsoft.Data.Sqlite.SqliteConnection>()));
            });
        }
    }
}
