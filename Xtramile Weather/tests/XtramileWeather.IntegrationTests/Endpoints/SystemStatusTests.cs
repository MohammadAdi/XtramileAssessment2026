using System.Net;
using System.Net.Http.Json;

namespace XtramileWeather.IntegrationTests.Endpoints;

public sealed class SystemStatusTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory factory;

    public SystemStatusTests(ApiWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetStatus_ReturnsReadyThroughTheApplicationPipeline()
    {
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/status");
        var payload = await response.Content.ReadFromJsonAsync<StatusPayload>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Ready", payload?.Status);
    }

    [Fact]
    public async Task GetStatus_AllowsTheConfiguredFrontendOrigin()
    {
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/status");
        request.Headers.Add("Origin", "http://localhost:5100");
        using var response = await client.SendAsync(request);

        Assert.Equal("http://localhost:5100",
            Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
    }

    private sealed record StatusPayload(string Status);
}
