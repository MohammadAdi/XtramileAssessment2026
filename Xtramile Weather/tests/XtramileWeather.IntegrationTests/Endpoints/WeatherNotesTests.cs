using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using XtramileWeather.Infrastructure.Persistence;

namespace XtramileWeather.IntegrationTests.Endpoints;

public sealed class WeatherNotesTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory factory;
    private readonly HttpClient client;

    public WeatherNotesTests(ApiWebApplicationFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateWeatherNote_ValidRequest_PersistsAndReturnsIdentifier()
    {
        using var response = await client.PostAsJsonAsync(
            "/api/weather/notes",
            new { cityId = 4, text = "  Heavy rain after lunch.  " });
        var payload = await response.Content.ReadFromJsonAsync<CreateWeatherNotePayload>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(payload);
        Assert.NotEqual(Guid.Empty, payload.Id);
        Assert.Equal($"/api/weather/notes/{payload.Id}", response.Headers.Location?.ToString());

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
        var persistedNote = await dbContext.WeatherNotes
            .AsNoTracking()
            .SingleAsync(note => note.Id == payload.Id);

        Assert.Equal(4, persistedNote.CityId);
        Assert.Equal("Heavy rain after lunch.", persistedNote.Text);
        Assert.Equal(TimeSpan.Zero, persistedNote.CreatedAtUtc.Offset);
    }

    [Fact]
    public async Task CreateWeatherNote_UnknownCity_ReturnsNotFound()
    {
        using var response = await client.PostAsJsonAsync(
            "/api/weather/notes",
            new { cityId = 999, text = "Rain expected." });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(0, "Rain expected.", "cityId")]
    [InlineData(4, "", "text")]
    public async Task CreateWeatherNote_InvalidRequest_ReturnsValidationProblem(
        int cityId,
        string text,
        string expectedField)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/weather/notes",
            new { cityId, text });
        using var problemDetails = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.True(problemDetails.RootElement
            .GetProperty("errors")
            .TryGetProperty(expectedField, out var errors));
        Assert.NotEqual(0, errors.GetArrayLength());
    }

    private sealed record CreateWeatherNotePayload(Guid Id);
}
