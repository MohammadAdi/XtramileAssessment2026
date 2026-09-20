using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace XtramileWeather.IntegrationTests.Endpoints;

public sealed class CountriesTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient client;

    public CountriesTests(ApiWebApplicationFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCountries_ReturnsCountryReferenceData()
    {
        var countries = await client.GetFromJsonAsync<CountryPayload[]>("/api/countries");

        Assert.NotNull(countries);
        Assert.Contains(countries, country => country.Code == "ID" && country.Name == "Indonesia");
    }

    [Fact]
    public async Task GetCities_ReturnsOnlyCitiesForSelectedCountry()
    {
        using var response = await client.GetAsync("/api/countries/id/cities");
        var payload = await response.Content.ReadFromJsonAsync<CountryCitiesPayload>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("ID", payload?.CountryCode);
        Assert.Equal("Indonesia", payload?.CountryName);
        Assert.NotNull(payload?.Cities);
        Assert.All(payload.Cities, city => Assert.InRange(city.Latitude, -90m, 90m));
        Assert.Contains(payload.Cities, city => city.Name == "Jakarta");
        Assert.DoesNotContain(payload.Cities, city => city.Name == "Sydney");
    }

    [Fact]
    public async Task GetCities_ReturnsNotFoundForUnknownCountry()
    {
        using var response = await client.GetAsync("/api/countries/ZZ/cities");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("I")]
    [InlineData("123")]
    public async Task GetCities_ReturnsValidationProblemForInvalidCountryCode(string countryCode)
    {
        using var response = await client.GetAsync($"/api/countries/{countryCode}/cities");
        using var problemDetails = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(400, problemDetails.RootElement.GetProperty("status").GetInt32());
        Assert.True(problemDetails.RootElement
            .GetProperty("errors")
            .TryGetProperty("countryCode", out var countryCodeErrors));
        Assert.NotEqual(0, countryCodeErrors.GetArrayLength());
    }

    private sealed record CountryPayload(string Code, string Name);

    private sealed record CountryCitiesPayload(
        string CountryCode,
        string CountryName,
        CityPayload[] Cities);

    private sealed record CityPayload(
        string Name,
        decimal Latitude,
        decimal Longitude);
}
